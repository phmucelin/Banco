namespace Banco.Models
{
    public class Conta
        {
            public string Titular { get; set; } = string.Empty;
            public double Saldo { get; set; }
            public List<Transacao> Historico { get; set; } = new List<Transacao>();
        }
}