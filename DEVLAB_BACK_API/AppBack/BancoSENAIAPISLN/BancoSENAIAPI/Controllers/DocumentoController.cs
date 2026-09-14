using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(), 
            "ClienteArquivos"
            );
        private static List<Models.DocumentoMetadado> _documentoMetadados = new List<Models.DocumentoMetadado>();
        
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<ActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi gerado.");
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string extensao = Path.GetExtension(arquivo.FileName);
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId++,
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };

            _documentoMetadados.Add(documentoMetadados);

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }
        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ListarPorCliente(int codigoCliente)
        {
            var documentos = _documentoMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();
            if (!documentos.Any())
            {
                return NotFound();
            }
            return Ok(documentos);
        }
        [HttpGet("download/{id}")]
        public IActionResult DownloadArquivo(int id)
        {
            var documento = _documentoMetadados.FirstOrDefault(d => d.Id == id);
            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }
            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound("Arquivo físico não encontrado no servidor.");
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);
            string nomeParaDownload = $"{documento.Name}{documento.Extensao}";
            return File(fileBytes, "application/octet-stream", nomeParaDownload);
        }
        [HttpDelete("excluir/{id}")]
        public IActionResult ExcluirDocumento(int id)
        {
            var documento = _documentoMetadados.FirstOrDefault(d => d.Id == id);
            if(documento == null)
            {
                return NotFound("Documento não encontrado.");
            }
            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }
            _documentoMetadados.Remove(documento);
            return Ok(new { mensagem = "Documento e arquivo físico excluídos com sucesso." });
        }
    }
}