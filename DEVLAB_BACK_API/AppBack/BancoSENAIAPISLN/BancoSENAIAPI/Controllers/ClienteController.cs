using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : Controller
    {
        public class ClienteController : ControllerBase
        {
            private static List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente { CodigoCliente = 1001, NomeCliente = "Sérgio", CPF = 1000000, NumeroAgencia = 74, SaldoTotal = 1000, Sexo = "Masculino", Endereço = "Pão", Cidade = "Aracaju", Estado = "SE" },
            new Cliente { CodigoCliente = 2002, NomeCliente = "Melissa", CPF = 1000000,  NumeroAgencia = 74, SaldoTotal = 1000, Sexo = "Feminino", Endereço = "Pão", Cidade = "Aracaju", Estado = "SE"},
            new Cliente { CodigoCliente = 3003, NomeCliente = "Amanda", CPF = 1000000,  NumeroAgencia = 74, SaldoTotal = 1000, Sexo = "feminino", Endereço = "Pão", Cidade = "Aracaju", Estado = "SE" }
        };

            public object clienteAtualizado { get; private set; }

            [HttpGet]
            public IActionResult ListarTodas()
            {
                return Ok(_clientes);
            }

            [HttpPost]
            public IActionResult Cadastrar([FromBody] Cliente novoCliente)
            {

                if (_clientes.Any(a => a.CodigoCliente == novoCliente.CodigoCliente))
                    return BadRequest(new { message = "Este código já existe." });

                _clientes.Add(novoCliente);
                // Retorna Status 201 Created conforme boas práticas REST [6, 8]
                return Created("", novoCliente);
            }

            [HttpGet("{codigo}")]
            public IActionResult ConsultarPorCodigo(int codigo)
            {
                var cliente = _clientes.FirstOrDefault(a => a.CodigoCliente == codigo);

                if (cliente == null)
                    return NotFound(new { message = "Cliente não encontrado." }); // Status 404 [6, 7]

                return Ok(cliente); // Status 200 OK [6, 7]
            }

            [HttpPut("{codigo}")]
            public IActionResult Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
            {
                var clienteExistente = _clientes.FirstOrDefault(a => a.CodigoCliente == codigo);

                if (clienteExistente == null) return NotFound();

                clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
                clienteExistente.CPF = clienteAtualizado.CPF;
                clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
                clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;
                clienteExistente.Sexo = clienteAtualizado.Sexo;
                clienteExistente.Endereço = clienteAtualizado.Endereço;
                clienteExistente.Cidade = clienteAtualizado.Cidade;
                clienteExistente.Estado = clienteAtualizado.Estado;

                // Retorna Status 204 No Content para atualizações bem-sucedidas [6, 9]
                return NoContent();
            }

            [HttpDelete("{codigo}")]
            public IActionResult Excluir(int codigo)
            {
                var cliente = _clientes.FirstOrDefault(a => a.CodigoCliente == codigo);

                if (cliente == null) return NotFound();

                _clientes.Remove(cliente);
                return Ok(new { message = "Cliente excluída com sucesso." }); // Status 200 [6]
            }
        }
    }
}
