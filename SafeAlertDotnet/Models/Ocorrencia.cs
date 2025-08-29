using System.Text.Json.Serialization;

namespace SafeAlertApi.Models;

public class Ocorrencia
{
    public int Id { get; set; }
    public int Postagem_id { get; set; }
    public string Status { get; set; } = null!;
    public DateTime Data_ocorrencia { get; set; } = DateTime.Now;

    [JsonIgnore]
    public Postagem Postagem { get; set; } = null!;
}