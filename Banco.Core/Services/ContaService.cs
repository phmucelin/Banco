using Banco.Models;
using Banco.Enums;

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

        public void Depositar(int id, double valor)
        {
            if (!_contas.ContainsKey(id))
                throw new ArgumentException("Conta não encontrada.");

            var conta = _contas[id];
            
            if (valor <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            conta.Saldo += valor;

            conta.Historico.Add(new Transacao
            {
                Tipo = TipoTransacao.Deposito,
                Origem = conta.Titular,
                Destino = null,
                Valor = valor,
                Data = DateTime.Now
            });
        }

        public void Saque(int id, double valor)
        {
            if (!_contas.ContainsKey(id))
                throw new ArgumentException("Conta não encontrada.");

            var conta = _contas[id];
            
            if (valor <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            if (valor > conta.Saldo)
                throw new InvalidOperationException("Saldo insuficiente.");

            conta.Saldo -= valor;

            conta.Historico.Add(new Transacao
            {
                Tipo = TipoTransacao.Saque,
                Origem = conta.Titular,
                Destino = null,
                Valor = valor,
                Data = DateTime.Now
            });
        }

        public void TransacaoPix(int idOrigem, int idDestino, double valor)
        {
            if (!_contas.ContainsKey(idOrigem))
                throw new ArgumentException("Conta de origem não encontrada.");
            
            if (!_contas.ContainsKey(idDestino))
                throw new ArgumentException("Conta de destino não encontrada.");

            var origem = _contas[idOrigem];
            var destino = _contas[idDestino];

            if (valor <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            if (origem.Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente.");

            origem.Saldo -= valor;
            destino.Saldo += valor;

            var transacao = new Transacao
            {
                Tipo = TipoTransacao.Pix,
                Origem = origem.Titular,
                Destino = destino.Titular,
                Valor = valor,
                Data = DateTime.Now
            };

            origem.Historico.Add(transacao);
            destino.Historico.Add(transacao);
        }
        static double MostrarSaldo(Conta conta)
        {
            return conta.Saldo;
        }

        public List<Conta> exibeAll()
            {
                if(!listaDeContas.Any())
                {
                    throw new InvalidOperationException("Nenhuma conta registrada no sistema.");
                }
                return listaDeContas;
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
