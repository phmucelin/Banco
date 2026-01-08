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
            try
            {
                _contaService.Depositar(id, dto.Valor);
                return Ok();            
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }
        }
        [HttpPost("{id:int}/saque")]
        public IActionResult Saque(int id, [FromBody] SaqueDto dto)
        {
            try
            {
                _contaService.Saque(id, dto.Valor);
                return Ok();
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }
        }
        [HttpPost("{id:int}/transferenciapix")]
        public IActionResult TransferenciaPix(int id, [FromBody] TransferenciaPixDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _contaService.TransacaoPix(id, dto.IdDestino, dto.Valor);
                return NoContent();
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }
        }
        [HttpGet("{id:int}/mostrarsaldo")]
        public IActionResult MostraSaldo(int id)
        {
            try
            {
                var saldo = _contaService.MostraSaldo(id);
                var dto = new SaldoDto
                {
                    ContaId = id,
                    Saldo = saldo
                };
                return Ok(dto);
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch(InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }
        }
        [HttpGet("contas")]
        public IActionResult TodasContas(int id, ListaDeContasDto dto)
        {
            try{
                var contas = _contaService.exibeAll();
                var dto = contas.Select(c=> new ContaDto
                {
                    Titular = c.Titular,
                    Saldo = c.Saldo
                });
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch(InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }

        }
        [HttpGet("filtrosaldo")]
        public IActionResult FiltrarSaldoConta([FromQuery] double min)
        {
            try
            {
                var contas = _contaService.FiltrarSaldoMinimo(min);
                return Ok(contas);
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch(InvalidOperationException e)
            {
                return UnprocessableEntity(e.Message);
            }
        }
        [HttpGet("por-titualr")]
        public IActionResult FindUser([FromQuery] string titular)
        {
            if (string.IsNullOrWhiteSpace(titular))
                return BadRequest("Titular é obrigatório.");

                var conta = _contaService.findUser(titular);
                if(conta == null)
                {
                    return NotFound("Conta nao encontrada.");
                }
                return Ok(conta);         
        }
    }
}