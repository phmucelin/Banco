namespace Banco.Api.Dtos
{
    public class ContaDto
    {
        public int Id { get; set; }
        public string Titular { get; set; } = string.Empty;
        public double Saldo { get; set; }
    }
}