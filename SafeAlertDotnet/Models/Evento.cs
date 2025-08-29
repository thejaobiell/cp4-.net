using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SafeAlertApi.Models;

public class Evento
{
    public int Id { get; set; }
    
    [MaxLength(4000)] 
    public string? Tipo { get; set; }

    [MaxLength(4000)]
    public string? Descricao { get; set; }

    [JsonIgnore]
    public ICollection<Postagem> Postagens { get; set; } = new List<Postagem>();
}