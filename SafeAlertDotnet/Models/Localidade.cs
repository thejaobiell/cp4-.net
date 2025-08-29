namespace SafeAlertApi.Models;
using System.Text.Json.Serialization;


public class Localidade
{
    public int Id { get; set; }
    public string Bairro { get; set; } = null!;
    public string Zona { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Postagem> Postagens { get; set; } = new List<Postagem>();
}