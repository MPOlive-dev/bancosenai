namespace BancoSENAIAPI.Dtos
{
    public class LoginPesponseDto
    {
        public required string Token { get; set; }
        public required DateTime ExpiraEm {  get; set; }
    }
}
