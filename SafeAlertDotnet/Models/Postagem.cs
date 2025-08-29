using System.Text.Json.Serialization;

namespace SafeAlertApi.Models;

public class Postagem
{
    public int Id { get; set; }
    public int Usuario_id { get; set; }
    public int Evento_id { get; set; }
    public int Localidade_id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Imagem_url { get; set; } = null!;
    public DateTime Data_criacao { get; set; } = DateTime.Now;

    [JsonIgnore]
    public Usuario Usuario { get; set; } = null!;
    
    [JsonIgnore]
    public Evento Evento { get; set; } = null!;
    
    [JsonIgnore]
    public Localidade Localidade { get; set; } = null!;
    
    [JsonIgnore]
    public ICollection<Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia>();
}