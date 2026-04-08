using System.Text.Json.Serialization;

namespace PontoMauiApp;

public class PontoRegistro
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("horario")]
    public DateTime Horario { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("observacao")]
    public string? Observacao { get; set; }
}