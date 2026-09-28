using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;

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
        
        private readonly AppDbContext _context;

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

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

            var novoDocumento = new Models.DocumentoMetadado
            {
                Nome = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };

            _context.Documento.Add(novoDocumento);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }
        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarPorCliente(int codigoCliente)
        {
            var documentos = await _context.Documento.ToListAsync();
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
            string nomeParaDownload = $"{documento.Nome}{documento.Extensao}";
            return File(fileBytes, "application/octet-stream", nomeParaDownload);
        }
        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> ExcluirDocumento(int id)
        {
            var documento = await _context.Documento.FirstOrDefaultAsync(a => a.Id == id);
            if(documento == null)
            {
                return NotFound("Documento não encontrado.");
            }
            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }
            _context.Documento.Remove(documento);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Documento e arquivo físico excluídos com sucesso." });
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }
            const long limiteTamanho = 2 * 1024 * 1024;
            if (arquivo.Length > limiteTamanho)
            {
                return BadRequest("O arquivo não pode ter mais de 2 MB.");
            }
            string[] extensoesPermitidas = { ".pdf", ".jpg", ".png" };
            string extensoes = Path.GetExtension(arquivo.FileName).ToLower();
            if (!extensoesPermitidas.Contains(extensoes))
            {
                return BadRequest("Extensão de arquivo não permitida. Apenas .pdf, .jpg e .png são aceitos.");
            }
            return Ok("Arquivo enviado com sucesso.");
        }
    }
}