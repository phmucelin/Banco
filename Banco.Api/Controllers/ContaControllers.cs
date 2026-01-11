using Microsoft.AspNetCore.Mvc;
using Banco.Services;
using Banco.Api.Dto;
using System.Diagnostics.Contracts;

namespace Banco.Api.Controllers
{
    [ApiController]
    [Route("api/contas")]
    public class ContasController : ControllerBase
    {
        private readonly ContaService _contaService;
        public ContasController(ContaService contaService)
        {
            _contaService = contaService;
        }
        [HttpPost]
        public IActionResult CriarConta([FromBody] CriarContaDto dto)
        {
            var conta = _contaService.Criar(dto.Titular);
            return Created("", conta);
        }
        [HttpPost("{id:int}/deposito")]
        public IActionResult Depositar(int id, [FromBody] DepositoDto dto)
        {
            var result = _contaService.Depositar(id, dto.Valor);
            if (!result.Success)
            {
                return UnprocessableEntity(result.Error);
            }
            return NoContent();
        }
        [HttpPost("{id:int}/saque")]
        public IActionResult Saque(int id, [FromBody] SaqueDto dto)
        {
            var result = _contaService.Sacar(id, dto.Valor);
            if (!result.Success)
            {
                return UnprocessableEntity(result.Error);
            }
            return NoContent();
        }
        [HttpPost("{id:int}/transferenciapix")]
        public IActionResult TransferenciaPix(int id, [FromBody] TransferenciaPixDto dto)
        {
            var result = _contaService.TransacaoPix(id, dto.Destino, dto.Valor);
            if (!result.Success)
            {
                return UnprocessableEntity(result.Error);
            }
            return NoContent();
        }
        [HttpGet("{id:int}/mostrarsaldo")]
        public IActionResult MostraSaldo(int id)
        {
            var result = _contaService.MostrarSaldo(id);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }

            var dto = new SaldoDto
            {
                ContaId = id,
                Saldo = result.Value
            };

            return Ok(dto);
        }

        [HttpGet("contas")]
        public IActionResult TodasContas(int id, ListaDeContasDto dto)
        {
            var result = _contaService.exibeAll();
            var dto = result.Value.Select(c=> new ContaDto
            {
                Titular = c.Titular,
                Saldo = c.Saldo
            });
            if (!result.Success)
            {
                return UnprocessableEntity(result.Error);
            }
            return Ok(dto);
        }
        [HttpGet("filtrosaldo")]
        public IActionResult FiltrarSaldoConta([FromQuery] double min)
        {

            var result = _contaService.FiltrarSaldoMinimo(min);
            if (!result.Success)
            {
                return UnprocessableEntity(result.Error);
            }
            return Ok(result.Data);
        }
        [HttpGet("por-titular")]
        public IActionResult FindUser([FromQuery] string titular)
        {
            if (string.IsNullOrWhiteSpace(titular))
                return BadRequest("Titular é obrigatório.");

                var result = _contaService.findUser(titular);
            if (!result.Success)
            {
                return UnproccessableEntity(result.Error);
            }
                return Ok(result.Data);         
        }
    }
}