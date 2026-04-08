namespace PontoAPI.Models;

public class PontoRegistro
{
    public long Id { get; set; }
    public DateTime Horario { get; set; } = DateTime.UtcNow;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Observacao { get; set; }
}