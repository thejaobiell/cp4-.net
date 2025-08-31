namespace SafeAlertApi.DTOs;

public class PostagemDto
{
    public int Id { get; set; }
    public int Usuario_id { get; set; }
    public int Evento_id { get; set; }
    public int Localidade_id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Imagem_url { get; set; } = null!;
    public DateTime Data_criacao { get; set; }  
}