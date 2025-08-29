namespace SafeAlertApi.DTOs;

public class OcorrenciaDTO
{
    public int Id { get; set; }
    public int Postagem_id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Data_ocorrencia { get; set; }
}
