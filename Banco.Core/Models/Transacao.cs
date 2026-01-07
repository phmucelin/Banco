using Banco.Enums;

namespace Banco.Models
{
    public class Transacao
    {
        public TipoTransacao Tipo { get; set; }
        public string Origem { get; set; } = string.Empty;
        public string? Destino { get; set; }
        public double Valor { get; set; }
        public DateTime Data { get; set; }
    }
}
