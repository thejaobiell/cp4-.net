using System.Text.Json.Serialization;

namespace SafeAlertApi.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!; 
    public string Endereco { get; set; } = null!;
    public string Tipo_usuario { get; set; } = null!;
    public DateTime Data_cadastro { get; set; } = DateTime.Now;

    [JsonIgnore]
    public ICollection<Postagem> Postagens { get; set; } = new List<Postagem>();
}