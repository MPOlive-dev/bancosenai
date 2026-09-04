namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        public int CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string CPF { get; set; }
        public int NumeroAgencia { get; set; }
        public int SaldoTotal { get; set; }
        public string Sexo { get; set; }
        public string Endereço { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set;}
    }
}
