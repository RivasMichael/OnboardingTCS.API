using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Security.Claims;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IOllamaService _ollamaService;

        public DocumentosController(IDocumentoRepository documentoRepository, IOllamaService ollamaService)
        {
            _documentoRepository = documentoRepository;
            _ollamaService = ollamaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documento>>> GetAll()
        {
            try
            {
                var documentos = await _documentoRepository.GetAllAsync();
                return Ok(documentos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Documentos] Error al obtener documentos: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener documentos" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Documento>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID del documento es requerido.");
                }

                var documento = await _documentoRepository.GetByIdAsync(id);
                if (documento == null)
                {
                    return NotFound($"Documento no encontrado con ID: {id}");
                }

                return Ok(documento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Documentos] Error al obtener documento {id}: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener documento" });
            }
        }

        [HttpPost("upload-complete")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadCompleteDocument([FromForm] CompleteDocumentUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("No se ha seleccionado ningun archivo.");
            }

            if (Path.GetExtension(request.File.FileName).ToLower() != ".pdf")
            {
                return BadRequest("El archivo debe ser un PDF valido.");
            }

            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                return BadRequest("El titulo es requerido.");
            }

            if (string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest("La descripcion es requerida.");
            }

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var adminName = User.FindFirst(ClaimTypes.Name)?.Value;

            Console.WriteLine($"[DocumentUploadComplete] {adminName} subiendo documento completo: {request.File.FileName}");

            var tempPath = Path.GetTempPath();
            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
            var filePath = Path.Combine(tempPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                var extractedText = ExtractTextFromPdf(filePath);

                if (string.IsNullOrEmpty(extractedText))
                {
                    return BadRequest("No se pudo extraer texto del PDF. Verifique que el archivo no este danado.");
                }

                var documento = new Documento
                {
                    Titulo = request.Titulo.Trim(),
                    Descripcion = request.Descripcion.Trim(),
                    Categoria = request.Categoria?.Trim() ?? "General",
                    NombreArchivo = request.File.FileName,
                    UrlArchivo = null,
                    TipoArchivo = "application/pdf",
                    TamanoArchivo = request.File.Length,
                    Obligatorio = request.Obligatorio,
                    SubidoPor = adminId ?? "admin",
                    SubidoPorNombre = adminName ?? "Administrador",
                    VisibleTodos = true, // Siempre true - no viene del frontend
                    Descargas = 0,
                    CreadoEn = DateTime.UtcNow, // Servidor controla la fecha
                    Archivo = extractedText
                };

                if (!string.IsNullOrEmpty(extractedText))
                {
                    try
                    {
                        var textForEmbedding = extractedText.Substring(0, Math.Min(extractedText.Length, 1000));
                        documento.Embedding = await _ollamaService.GenerateEmbeddingAsync(textForEmbedding);
                        Console.WriteLine($"[DocumentUploadComplete] Embeddings generados para '{documento.Titulo}'");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DocumentUploadComplete] Error generando embeddings: {ex.Message}");
                    }
                }

                await _documentoRepository.CreateAsync(documento);

                Console.WriteLine($"[DocumentUploadComplete] Documento '{documento.Titulo}' creado exitosamente. ID: {documento.Id}");

                return Ok(new
                {
                    mensaje = "Documento subido y procesado correctamente.",
                    documento = new
                    {
                        id = documento.Id,
                        titulo = documento.Titulo,
                        descripcion = documento.Descripcion,
                        categoria = documento.Categoria,
                        obligatorio = documento.Obligatorio,
                        visibleTodos = documento.VisibleTodos,
                        nombreArchivo = documento.NombreArchivo,
                        tamano = documento.TamanoArchivo,
                        caracteres_extraidos = extractedText.Length,
                        tiene_embeddings = documento.Embedding != null,
                        creado_en = documento.CreadoEn,
                        subido_por = documento.SubidoPorNombre
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentUploadComplete] Error procesando documento: {ex.Message}");
                return StatusCode(500, new { error = $"Error procesando el documento: {ex.Message}" });
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        [HttpPost("upload")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha seleccionado ningun archivo.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
            {
                return BadRequest("El archivo debe ser un PDF valido.");
            }

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var adminName = User.FindFirst(ClaimTypes.Name)?.Value;

            Console.WriteLine($"[DocumentUpload] {adminName} subiendo archivo: {file.FileName} ({file.Length} bytes)");

            var tempPath = Path.GetTempPath();
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(tempPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var extractedText = ExtractTextFromPdf(filePath);

                if (string.IsNullOrEmpty(extractedText))
                {
                    return BadRequest("No se pudo extraer texto del PDF. Verifique que el archivo no este danado.");
                }

                var documento = new Documento
                {
                    Titulo = Path.GetFileNameWithoutExtension(file.FileName),
                    Descripcion = "Documento pendiente de completar informacion",
                    Categoria = "Sin categorizar",
                    NombreArchivo = file.FileName,
                    UrlArchivo = null,
                    TipoArchivo = "application/pdf",
                    TamanoArchivo = file.Length,
                    Obligatorio = false,
                    SubidoPor = adminId ?? "admin",
                    SubidoPorNombre = adminName ?? "Administrador",
                    VisibleTodos = true,
                    Descargas = 0,
                    CreadoEn = DateTime.UtcNow,
                    Archivo = extractedText
                };

                await _documentoRepository.CreateAsync(documento);

                Console.WriteLine($"[DocumentUpload] Archivo '{file.FileName}' procesado exitosamente. ID: {documento.Id}");

                return Ok(new { 
                    mensaje = "Archivo PDF subido y procesado correctamente.",
                    documentoId = documento.Id,
                    nombreArchivo = file.FileName,
                    tamano = file.Length,
                    caracteres_extraidos = extractedText.Length,
                    proximo_paso = $"Completar informacion con POST /api/documentos/{documento.Id}/complete"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentUpload] Error procesando archivo: {ex.Message}");
                return StatusCode(500, new { error = $"Error procesando el archivo: {ex.Message}" });
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CompleteDocumentInfo(string id, [FromBody] CompleteDocumentRequest request)
        {
            try
            {
                Console.WriteLine($"[DocumentComplete] Recibida request para documento ID: {id}");

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID del documento es requerido.");
                }

                if (request == null)
                {
                    return BadRequest("Los datos del documento son requeridos.");
                }

                var documento = await _documentoRepository.GetByIdAsync(id);
                if (documento == null)
                {
                    Console.WriteLine($"[DocumentComplete] Documento no encontrado con ID: {id}");
                    return NotFound($"Documento no encontrado con ID: {id}");
                }

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var adminName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                Console.WriteLine($"[DocumentComplete] Admin: {adminName} ({adminId}) actualizando documento");

                documento.Titulo = request.Titulo;
                documento.Descripcion = request.Descripcion;
                documento.Categoria = request.Categoria;
                documento.Obligatorio = request.Obligatorio;
                documento.VisibleTodos = true;

                if (!string.IsNullOrEmpty(documento.Archivo) && documento.Embedding == null)
                {
                    try
                    {
                        var textForEmbedding = documento.Archivo.Substring(0, Math.Min(documento.Archivo.Length, 1000));
                        documento.Embedding = await _ollamaService.GenerateEmbeddingAsync(textForEmbedding);
                        Console.WriteLine($"[DocumentComplete] Embeddings generados para '{documento.Titulo}'");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DocumentComplete] Error generando embeddings: {ex.Message}");
                    }
                }

                await _documentoRepository.UpdateAsync(id, documento);

                Console.WriteLine($"[DocumentComplete] Informacion completada para documento '{documento.Titulo}'");

                return Ok(new { 
                    mensaje = "Informacion del documento completada exitosamente.",
                    documento = new {
                        id = documento.Id,
                        titulo = documento.Titulo,
                        categoria = documento.Categoria,
                        obligatorio = documento.Obligatorio,
                        tiene_embeddings = documento.Embedding != null
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentComplete] Error: {ex.Message}");
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID del documento es requerido.");
                }

                var documento = await _documentoRepository.GetByIdAsync(id);
                if (documento == null)
                {
                    return NotFound($"Documento no encontrado con ID: {id}");
                }

                await _documentoRepository.DeleteAsync(id);

                Console.WriteLine($"[DocumentDelete] Documento '{documento.Titulo}' eliminado");

                return Ok(new { mensaje = "Documento eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentDelete] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al eliminar documento" });
            }
        }

        private string ExtractTextFromPdf(string filePath)
        {
            try
            {
                using var pdfReader = new PdfReader(filePath);
                using var pdfDocument = new PdfDocument(pdfReader);
                var text = new System.Text.StringBuilder();

                for (var i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i));
                    text.Append(pageText);
                }

                return text.ToString().Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PDFExtract] Error extrayendo texto: {ex.Message}");
                return string.Empty;
            }
        }
    }

    public class CompleteDocumentUploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Categoria { get; set; } = "General";
        public bool Obligatorio { get; set; } = false;
        // VisibleTodos removido - siempre será true por defecto
    }

    public class CompleteDocumentRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = "General";
        public bool Obligatorio { get; set; } = false;
        public bool VisibleTodos { get; set; } = true;
    }
}