using Banco.Models;
using Banco.Enums;
using Banco.Result;
using System.IO.Pipelines;

namespace Banco.Services
{
    public class ContaService
    {
        private readonly Dictionary<int, Conta> _contas = new();
        private int _proximoId = 1;

        public Conta Criar(string titular)
        {
            var conta = new Conta
            {
                Titular = titular,
                Saldo = 0
            };
            
            var id = _proximoId++;
            _contas[id] = conta;
            return conta;
        }

        public Result Depositar(int id, double valor)
        {
            if (!_contas.ContainsKey(id))
                return Result.Fail("Conta inexistente.");

            var conta = _contas[id];
            
            if (valor <= 0)
                return Result.Fail("Valor invalido.");

            conta.Saldo += valor;

            conta.Historico.Add(new Transacao
            {
                Tipo = TipoTransacao.Deposito,
                Origem = conta.Titular,
                Destino = null,
                Valor = valor,
                Data = DateTime.Now
            });
            return Result.Ok();
        }

        public Result Saque(int id, double valor)
        {
            if (!_contas.ContainsKey(id))
                return Result.Fail("Conta nao encontrada.");

            var conta = _contas[id];
            
            if (valor <= 0)
                return Result.Fail("Error -> valor deve ser superior e diferente de zero");

            if (valor > conta.Saldo)
                return Result.Fail("Saldo insuficiente.");

            conta.Saldo -= valor;

            conta.Historico.Add(new Transacao
            {
                Tipo = TipoTransacao.Saque,
                Origem = conta.Titular,
                Destino = null,
                Valor = valor,
                Data = DateTime.Now
            });
            return Result.Ok();
        }

        public Result TransacaoPix(int idOrigem, int idDestino, double valor)
        {
            if (!_contas.ContainsKey(idOrigem))
                return Result.Fail("Conta de origem nao encontrada.");
            
            if (!_contas.ContainsKey(idDestino))
                return Result.Fail("Conta de destino nao encontrada.");

            var origem = _contas[idOrigem];
            var destino = _contas[idDestino];

            if (valor <= 0)
                return Result.Fail("Error -> valor deve ser superior e diferente de zero");

            if (origem.Saldo < valor)
                return Result.Fail("Saldo insuficiente.");

            origem.Saldo -= valor;
            destino.Saldo += valor;

            origem.Historico.Add( new Transacao
            {
                Tipo = TipoTransacao.Pix,
                Origem = origem.Titular,
                Destino = destino.Titular,
                Valor = valor,
                Data = DateTime.Now
            });
            destino.Historico.Add( new Transacao
            {
                Tipo = TipoTransacao.Pix,
                Origem = origem.Titular,
                Destino = destino.Titular,
                Valor = valor,
                Data = DateTime.Now
            });
            return Result.Ok();
        }
        public Result<double> MostrarSaldo(int id)
        {
            if (!_contas.ContainsKey(id))
            {
                return Result<double>.Fail("Conta inexistente.");
            }

            var conta = _contas[id];

            return Result<double>.Ok(conta.Saldo);
        }

        public Result<List<Conta>> ExibeTodas()
            {
                var listaDeContas = Conta.ToList();
                if(!Conta.Any())
                {
                    return Result<List<Conta>>.Fail("Nao existem contas previamente cadastradas no sistema.");
                }
                return Result<List<Conta>>.Ok(listaDeContas);
            }
        
        public Result<List<Conta>> existeNegativados()
            {
                var check = listaDeContas.Where(t => t.Saldo < 0).ToList();
                if (!check.Any())
                {
                    return Result<List<Conta>>.Fail("Nenhuma conta com saldo negativado.");
                }
                return Result<List<Conta>>.Ok(check);
            }

        public Result<List<Conta>> FiltroSaldoConta(double vmin)
            {
                if (vmin < 0)
                {
                    return Result<List<Conta>>.Fail("Valor minimo invalido");
                }
                var contas = listaDeContas.Where(t=> t.Saldo > vmin).ToList();
                if (!contas.Any())
                {
                    return Result<List<Conta>>.Fail("Nenhuma conta encontrada.");
                }
                return Result<List<Conta>>.Ok(contas);
            }

        public Result<Conta> findUser(string titular)
            {
                var finding = listaDeContas.FirstOrDefault(t => t.Titular == titular);
                if(finding == null)
                {
                    return Result<Conta>.Fail("Conta nao encontrada");
                }
                return Result<Conta>.Ok(finding);
            }

        public Result<double> SomaTotal()
            {
                double somaTotal = 0;
                if(!listaDeContas.Any()) return Result<double>.Fail("Nenhuma conta cadastrada");
                foreach(var conta in listaDeContas)
                {
                    somaTotal += conta.Saldo;
                }
                return Result<double>.Ok(somaTotal);
            }
        public Result<List<Transacao>> filtraHistoricoGeral(Conta conta)
            {
                if (!conta.Historico.Any())
                {
                    return Result<List<Transacao>>.Fail("Nenhum historico nesta conta.");
                }   
                return Result<List<Transacao>>.Ok(conta.Historico);
            }
        public Result<List<Conta>> FiltraHistoricoTipo(Conta conta, Enum TipoTransacao)
            {
                if (!conta.Historico.Any())
                {
                    return Result<List<Conta>>.Fail("Nenhum historico nessa conta.");
                }
                if(!conta.Historico.Where(t=> t.Tipo == TipoTransacao))
                {
                    return Result<List<Conta>>.Fail("Nenhum historico deste tipo na conta.");
                }
                var buscando = conta.Historico.Where(t=> t.Tipo == TipoTransacao);
                return Result<List<Conta>>.Ok(buscando);
            }
    }
}
