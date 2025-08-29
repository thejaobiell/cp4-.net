namespace SafeAlertApi.DTOs;

public class EventoDTO
{
    public int Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Descricao { get; set; } = null!;   
}