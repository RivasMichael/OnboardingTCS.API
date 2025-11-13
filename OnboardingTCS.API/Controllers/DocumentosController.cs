using Microsoft.AspNetCore.Mvc;
using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace OnboardingTCS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var documentos = await _documentoRepository.GetAllAsync();
            return Ok(documentos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Documento>> GetById(string id)
        {
            var documento = await _documentoRepository.GetByIdAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            return Ok(documento);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Documento documento)
        {
            await _documentoRepository.CreateAsync(documento);
            return CreatedAtAction(nameof(GetById), new { id = documento.Id }, documento);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Documento documento)
        {
            var existingDocumento = await _documentoRepository.GetByIdAsync(id);
            if (existingDocumento == null)
            {
                return NotFound();
            }
            documento.Id = id;
            await _documentoRepository.UpdateAsync(id, documento);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingDocumento = await _documentoRepository.GetByIdAsync(id);
            if (existingDocumento == null)
            {
                return NotFound();
            }
            await _documentoRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0 || Path.GetExtension(file.FileName).ToLower() != ".pdf")
            {
                return BadRequest("El archivo debe ser un PDF válido.");
            }

            // Guardar el archivo temporalmente
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extraer texto del PDF
            var extractedText = ExtractTextFromPdf(filePath);

            // Guardar en MongoDB (sin embeddings)
            var documento = new Documento
            {
                Titulo = Path.GetFileNameWithoutExtension(file.FileName),
                Descripcion = "Texto extraído del PDF",
                Categoria = "PDF",
                NombreArchivo = file.FileName,
                UrlArchivo = null, // Aquí puedes agregar la URL si decides usar almacenamiento externo
                TipoArchivo = "application/pdf",
                TamanoArchivo = file.Length,
                Obligatorio = false,
                SubidoPor = "Usuario", // Cambiar según el usuario autenticado
                SubidoPorNombre = "Nombre del Usuario", // Cambiar según el usuario autenticado
                VisibleTodos = true,
                Descargas = 0,
                CreadoEn = DateTime.UtcNow,
                Archivo = extractedText
            };

            await _documentoRepository.CreateAsync(documento);

            return Ok(new { mensaje = "PDF subido y procesado correctamente.", documentoId = documento.Id });
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPdf(string id)
        {
            var documento = await _documentoRepository.GetByIdAsync(id);
            if (documento == null)
            {
                return NotFound("Documento no encontrado.");
            }

            if (string.IsNullOrWhiteSpace(documento.NombreArchivo))
            {
                return BadRequest("El documento no tiene un archivo asociado.");
            }

            // Ruta del archivo PDF (puedes ajustar según tu almacenamiento)
            var filePath = Path.Combine(Path.GetTempPath(), documento.NombreArchivo);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("El archivo PDF no se encuentra en el servidor.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", documento.NombreArchivo);
        }

        public class PreguntaRequest
        {
            public string Pregunta { get; set; } = string.Empty;
        }

        [HttpPost("{id}/preguntar")]
        public async Task<IActionResult> Preguntar(string id, [FromBody] PreguntaRequest request)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

            // Obtener el documento desde MongoDB
            var documento = await _documentoRepository.GetByIdAsync(id);
            if (documento == null)
            {
                return NotFound("Documento no encontrado.");
            }

            // Enviar pregunta y contexto a Ollama
            var contexto = documento.Archivo; // Texto extraído del PDF
            var prompt = $"Contexto: {contexto}\n\nPregunta: {request.Pregunta}";
            var respuesta = await _ollamaService.GenerateResponseAsync(prompt);

            // Devolver la respuesta
            return Ok(new { respuesta });
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
                    text.Append(PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i)));
                }

                return text.ToString();
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return string.Empty;
            }
        }
    }
}