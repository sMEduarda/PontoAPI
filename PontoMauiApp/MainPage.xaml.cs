using System.Text.Json;
using System.Collections.ObjectModel;

namespace PontoMauiApp;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public ObservableCollection<PontoRegistro> Registros { get; set; } = new();

    public MainPage(HttpClient httpClient)
    {
        InitializeComponent();
        BindingContext = this;

        _httpClient = httpClient;

        _ = CarregarRegistrosAsync();
    }

    // ==================== ENTRADA ====================
    private async void BtnEntrada_Clicked(object sender, EventArgs e)
    {
        await RegistrarPonto("Entrada");
    }

    // ==================== SAÍDA ====================
    private async void BtnSaida_Clicked(object sender, EventArgs e)
    {
        await RegistrarPonto("Saída");
    }

    private async Task RegistrarPonto(string tipo)
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permissão", "Localização é obrigatória.", "OK");
                return;
            }

            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10)));

            if (location == null)
            {
                await DisplayAlert("Erro", "Não foi possível obter a localização.", "OK");
                return;
            }

            var registro = new PontoRegistro
            {
                Horario = DateTime.UtcNow,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Observacao = $"{tipo} - {DateTime.Now:HH:mm}"
            };

            var json = JsonSerializer.Serialize(registro);

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/PontoRegistros", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sucesso", $"{tipo} registrada com sucesso!", "OK");
                await CarregarRegistrosAsync();
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro API", erro, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"{ex.Message}\n\n{ex.InnerException?.Message}", "OK");
        }
    }

    private async Task CarregarRegistrosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/PontoRegistros");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var lista = JsonSerializer.Deserialize<List<PontoRegistro>>(json);

                Registros.Clear();

                foreach (var item in lista.OrderByDescending(r => r.Horario))
                    Registros.Add(item);
            }
        }
        catch
        {
            // API pode estar offline
        }
    }

    // ==================== GOOGLE MAPS ====================
    private async void VerNoMapa_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is PontoRegistro reg)
        {
            var lat = reg.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lon = reg.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var url = $"https://www.google.com/maps?q={lat},{lon}";

            await Launcher.OpenAsync(new Uri(url));
        }
    }
}