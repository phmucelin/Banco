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
        
        public List<Conta> existeNegativados()
            {
                var check = listaDeContas.Where(t => t.Saldo < 0).ToList();
                if (!listaDeContas.Any(t=> t.Saldo < 0))
                {
                    throw new InvalidOperationException("Nao encontramos nenhuma conta com saldo negativo.");
                }
                return check;
            }

        public List<Conta> FiltroSaldoConta(double vmin)
            {
                if (vmin < 0)
                {
                    throw new ArgumentException("Valor minimo invalido.");
                }
                var contas = listaDeContas.Where(t=> t.Saldo > vmin).ToList();
                if (!contas.Any())
                {
                    throw new InvalidOperationException("Nenhuma conta foi encontrada.");
                }
                return contas;
            }

        public Conta findUser(string Titular)
            {
                var finding = listaDeContas.FirstOrDefault(t => t.Titular == Titular);
                if(finding == null)
                {
                    throw new ArgumentException("O nome foi invalido.");
                }
                return finding;
            }

        public double SomaTotal()
            {
                double somaTotal = 0;
                foreach(var conta in listaDeContas)
                {
                    somaTotal += conta.Saldo;
                }
                return somaTotal;
            }
        public List<Transacao> filtraHistoricoGeral(Conta conta)
            {
                if (!conta.Historico.Any())
                {
                    throw new InvalidOperationException("Nenhum historico na conta");
                }   
                return conta.Historico;
            }
        public List<Conta> FiltraHistoricoTipo(Conta conta, Enum TipoTransacao)
            {
                if (!conta.Historico.Any())
                {
                    throw new ArgumentException("Nenhum historico presente na conta.");
                }
                if(!conta.Historico.Where(t=> t.Tipo == TipoTransacao))
                {
                    throw new InvalidOperationException("Nenhuma transacao encontrada com este tipo.");
                }
                var buscando = conta.Historico.Where(t=> t.Tipo == TipoTransacao);
                return buscando;
            }
    }
}
