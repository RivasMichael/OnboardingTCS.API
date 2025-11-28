using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Core.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Security.Claims;
using System.Linq;

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
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var documentos = await _documentoRepository.GetAllAsync();
                
                // Mapear incluyendo ID para descarga
                var documentosDto = documentos.Select(d => new
                {
                    id = d.Id,                    // ? INCLUIR ID para descarga
                    titulo = d.Titulo,
                    descripcion = d.Descripcion,
                    categoria = d.Categoria,
                    tamanoArchivo = d.TamanoArchivo,
                    creadoEn = d.CreadoEn
                });
                
                return Ok(documentosDto);
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

        [HttpGet("{id}/download")]
        public async Task<ActionResult> DownloadDocument(string id)
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

                var usuario = User.FindFirst(ClaimTypes.Name)?.Value;
                Console.WriteLine($"[DocumentDownload] Usuario {usuario} descargando documento: {documento.Titulo}");

                // Verificar si el documento tiene una URL de archivo almacenada
                if (!string.IsNullOrEmpty(documento.UrlArchivo) && System.IO.File.Exists(documento.UrlArchivo))
                {
                    // Incrementar contador de descargas
                    documento.Descargas++;
                    await _documentoRepository.UpdateAsync(id, documento);

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(documento.UrlArchivo);
                    var fileName = documento.NombreArchivo ?? $"{documento.Titulo}.pdf";
                    
                    Console.WriteLine($"[DocumentDownload] Archivo descargado: {fileName} ({fileBytes.Length} bytes)");
                    
                    return File(fileBytes, "application/pdf", fileName);
                }
                else
                {
                    // Si no hay archivo físico, generar PDF desde el texto extraído
                    Console.WriteLine($"[DocumentDownload] Archivo físico no encontrado. Generando desde texto extraído.");
                    
                    if (string.IsNullOrEmpty(documento.Archivo))
                    {
                        return NotFound("No hay contenido disponible para descargar.");
                    }

                    // Aquí podrías generar un PDF desde el texto, pero por simplicidad devolvemos el texto
                    var textBytes = System.Text.Encoding.UTF8.GetBytes(documento.Archivo);
                    var textFileName = $"{documento.Titulo}.txt";
                    
                    // Incrementar contador de descargas
                    documento.Descargas++;
                    await _documentoRepository.UpdateAsync(id, documento);
                    
                    Console.WriteLine($"[DocumentDownload] Contenido de texto descargado: {textFileName}");
                    
                    return File(textBytes, "text/plain", textFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentDownload] Error descargando documento {id}: {ex.Message}");
                return StatusCode(500, new { error = "Error al descargar el documento" });
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

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var adminName = User.FindFirst(ClaimTypes.Name)?.Value;

                Console.WriteLine($"[DocumentDelete] Admin {adminName} ({adminId}) eliminando documento: {documento.Titulo}");

                // Eliminar archivo físico si existe
                if (!string.IsNullOrEmpty(documento.UrlArchivo) && System.IO.File.Exists(documento.UrlArchivo))
                {
                    try
                    {
                        System.IO.File.Delete(documento.UrlArchivo);
                        Console.WriteLine($"[DocumentDelete] Archivo físico eliminado: {documento.UrlArchivo}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DocumentDelete] Error eliminando archivo físico: {ex.Message}");
                    }
                }

                await _documentoRepository.DeleteAsync(id);
                Console.WriteLine($"[DocumentDelete] Documento '{documento.Titulo}' eliminado exitosamente por {adminName}");

                return Ok(new { 
                    mensaje = "Documento eliminado exitosamente.",
                    documento_eliminado = new {
                        id = documento.Id,
                        titulo = documento.Titulo,
                        categoria = documento.Categoria,
                        eliminado_por = adminName,
                        eliminado_en = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentDelete] Error al eliminar documento {id}: {ex.Message}");
                return StatusCode(500, new { error = "Error al eliminar el documento" });
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

            // Crear directorio para archivos si no existe
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "documents");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            // Generar nombre único para el archivo
            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
            var filePath = Path.Combine(uploadsDir, fileName);

            try
            {
                // 1. GUARDAR EL PDF FÍSICAMENTE
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // 2. EXTRAER TEXTO DEL PDF
                var extractedText = ExtractTextFromPdf(filePath);

                if (string.IsNullOrEmpty(extractedText))
                {
                    return BadRequest("No se pudo extraer texto del PDF. Verifique que el archivo no este danado.");
                }

                // 3. CREAR REGISTRO EN BASE DE DATOS
                var documento = new Documento
                {
                    Titulo = request.Titulo.Trim(),
                    Descripcion = request.Descripcion.Trim(),
                    Categoria = request.Categoria?.Trim() ?? "General",
                    NombreArchivo = request.File.FileName,
                    UrlArchivo = filePath, // ? GUARDA LA RUTA DEL ARCHIVO FÍSICO
                    TipoArchivo = "application/pdf",
                    TamanoArchivo = request.File.Length,
                    Obligatorio = request.Obligatorio,
                    SubidoPor = adminId ?? "admin",
                    SubidoPorNombre = adminName ?? "Administrador",
                    VisibleTodos = true,
                    Descargas = 0,
                    CreadoEn = DateTime.UtcNow,
                    Archivo = extractedText // ? TAMBIÉN GUARDA EL TEXTO EXTRAÍDO
                };

                // 4. GENERAR EMBEDDINGS PARA IA
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

                Console.WriteLine($"[DocumentUploadComplete] Documento '{documento.Titulo}' guardado en: {filePath}");

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
                        subido_por = documento.SubidoPorNombre,
                        archivo_guardado_en = filePath
                    }
                });
            }
            catch (Exception ex)
            {
                // Si hay error, eliminar archivo físico si se creó
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch { }
                }

                Console.WriteLine($"[DocumentUploadComplete] Error procesando documento: {ex.Message}");
                return StatusCode(500, new { error = $"Error procesando el documento: {ex.Message}" });
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

        /// <summary>
        /// Obtener documentos filtrados por categoría
        /// </summary>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult> GetByCategoria(string categoria)
        {
            try
            {
                var documentos = await _documentoRepository.GetAllAsync();
                
                // Filtrar por categoría (case insensitive)
                IEnumerable<Documento> documentosFiltrados;
                
                if (string.IsNullOrEmpty(categoria) || categoria.ToLower() == "todos")
                {
                    // Si es "todos" devolver todos los documentos
                    documentosFiltrados = documentos;
                }
                else
                {
                    // Filtrar por categoría específica
                    documentosFiltrados = documentos.Where(d => 
                        d.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
                }

                // Mapear resultado incluyendo ID para descarga
                var documentosDto = documentosFiltrados.Select(d => new
                {
                    id = d.Id,
                    titulo = d.Titulo,
                    descripcion = d.Descripcion,
                    categoria = d.Categoria,
                    tamanoArchivo = d.TamanoArchivo,
                    creadoEn = d.CreadoEn
                });
                
                Console.WriteLine($"[DocumentosCategoria] Filtro '{categoria}': {documentosDto.Count()} documentos encontrados");
                
                return Ok(documentosDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentosCategoria] Error al filtrar por categoría {categoria}: {ex.Message}");
                return StatusCode(500, new { error = "Error al filtrar documentos por categoría" });
            }
        }

        /// <summary>
        /// Obtener lista de todas las categorías disponibles
        /// </summary>
        [HttpGet("categorias")]
        public async Task<ActionResult> GetCategorias()
        {
            try
            {
                var documentos = await _documentoRepository.GetAllAsync();
                
                // Obtener categorías únicas
                var categorias = documentos
                    .Where(d => !string.IsNullOrEmpty(d.Categoria))
                    .Select(d => d.Categoria)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                // Agregar "Todos" al inicio
                categorias.Insert(0, "Todos");
                
                Console.WriteLine($"[DocumentosCategorias] {categorias.Count} categorías encontradas: {string.Join(", ", categorias)}");
                
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentosCategorias] Error al obtener categorías: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener categorías" });
            }
        }

        /// <summary>
        /// ?? API PARA PREGUNTAR SOBRE UN PDF ESPECÍFICO USANDO IA
        /// </summary>
        /// <param name="id">ID del documento PDF a consultar</param>
        /// <param name="request">Pregunta sobre el documento</param>
        /// <returns>Respuesta de la IA basada en el contenido del PDF</returns>
        [HttpPost("{id}/preguntar")]
        public async Task<IActionResult> PreguntarDocumento(string id, [FromBody] PreguntaDocumentoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

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

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                
                Console.WriteLine($"[DocumentoPregunta] Usuario {userName} pregunta sobre '{documento.Titulo}': {request.Pregunta.Substring(0, Math.Min(request.Pregunta.Length, 100))}...");

                // Verificar que el documento tenga contenido
                string textoDocumento = documento.Archivo ?? "";
                if (string.IsNullOrEmpty(textoDocumento))
                {
                    return BadRequest("El documento no tiene contenido disponible para consultar.");
                }

                // ?? Crear contexto especializado para la IA
                // ?? Crear contexto especializado para la IA
                var promptContextual = $@"Eres un asistente de onboarding muy amable, cercano y motivador.
Estás hablando con la persona llamada: {userName}.
Tu objetivo es que se sienta acompañado, valorado y con ganas de seguir aprendiendo.

DATOS DEL DOCUMENTO:
- Título: {documento.Titulo}
- Descripción: {documento.Descripcion}
- Categoría: {documento.Categoria}
- Tamaño: {FormatFileSize(documento.TamanoArchivo)}

CONTENIDO DEL DOCUMENTO (solo úsalo como contexto, no lo copies tal cual):
{textoDocumento}

PREGUNTA DEL USUARIO:
{request.Pregunta}

REGLAS DE ESTILO (MUY IMPORTANTES):
1. Responde SIEMPRE en español, usando un tono cercano, cálido y respetuoso.
2. Llama al usuario por su nombre al inicio o al final de la respuesta cuando sea natural.
3. Usa de 1 a 3 emojis por respuesta (no más), para que se vea amigable 😊 (ejemplos: 🙂✨👍🙌💡).
4. Responde breve: máximo de 2 a 4 oraciones. No des textos largos.
5. No copies párrafos enteros del documento; explica con tus propias palabras.
6. Si el usuario pide un resumen, da como máximo 4–5 viñetas cortas y claras.
7. Si la información no está en el documento, dilo con sinceridad y sugiere qué podría hacer.
8. Siempre termina con una frase de ánimo o invitación a seguir preguntando
   (por ejemplo: ""Si quieres, pregúntame otra cosa 😄"" o algo similar).

Ahora, con base en el documento y la pregunta del usuario, responde cumpliendo TODAS las reglas anteriores.";


                var respuesta = await _ollamaService.GenerateResponseAsync(promptContextual);
                
                Console.WriteLine($"[DocumentoPregunta] Respuesta generada para {userName} sobre documento '{documento.Titulo}'");
                
                return Ok(new { 
                    respuesta = respuesta,
                    documento = new {
                        id = documento.Id,
                        titulo = documento.Titulo,
                        categoria = documento.Categoria,
                        descripcion = documento.Descripcion,
                        tamano = FormatFileSize(documento.TamanoArchivo)
                    },
                    pregunta = request.Pregunta,
                    usuario = userName,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentoPregunta] Error: {ex.Message}");
                return StatusCode(500, new { error = "Error al procesar la consulta del documento" });
            }
        }

        /// <summary>
        /// Función auxiliar para formatear el tamaño de archivo
        /// </summary>
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    public class CompleteDocumentUploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Categoria { get; set; } = "General";
        public bool Obligatorio { get; set; } = false;
    }

    public class PreguntaDocumentoRequest
    {
        public string Pregunta { get; set; } = string.Empty;
    }
}