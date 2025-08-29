namespace SafeAlertApi.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Tipo_usuario { get; set; } = "";
        public DateTime Data_cadastro { get; set; }
    }
}