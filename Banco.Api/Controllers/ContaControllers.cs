using Microsoft.AspNetCore.Mvc;
using Banco.Services;
using Banco.Api.Dto;

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
    }
}