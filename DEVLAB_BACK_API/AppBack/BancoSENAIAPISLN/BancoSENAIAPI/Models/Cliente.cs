using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        [Key]
        public int CodigoCliente { get; set; }
        [Required]
        public string NomeCliente { get; set; }
        [Required]
        public int CPF { get; set; }
        public int NumeroAgencia { get; set; }
        [Required]
        public int SaldoTotal { get; set; }
        public string Sexo { get; set; }
        public string Endereço { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set;}
    }
}
