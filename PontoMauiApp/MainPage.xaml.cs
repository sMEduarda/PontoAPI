using Mapsui;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Features;
using System.Text;
using System.Text.Json;
using PontoMauiApp.Models;

namespace PontoMauiApp;

public partial class MainPage : ContentPage
{
    private const string ApiBaseUrl = "http://10.0.2.2:7000/";
    private readonly HttpClient _httpClient;

    private List<PontoRegistro> registros = new();

    public MainPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiBaseUrl)
        };

        // 🔥 Inicializa mapa OpenStreetMap
        var map = new Mapsui.Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        mapa.Map = map;

        //  _ = CarregarRegistros();
    }

    private async Task CarregarRegistros()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/PontoRegistros");

            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();

            registros = JsonSerializer.Deserialize<List<PontoRegistro>>(json);

            listaRegistros.ItemsSource = registros;

            // 🔥 Limpa e recria mapa base
            mapa.Map.Layers.Clear();
            mapa.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            foreach (var item in registros)
            {
                var point = SphericalMercator.FromLonLat(item.Longitude, item.Latitude);

                var position = new MPoint(point.x, point.y);

                var feature = new PointFeature(position);

                feature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.8
                });

                var layer = new MemoryLayer
                {
                    Features = new[] { feature }
                };

                mapa.Map.Layers.Add(layer);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao carregar", ex.Message, "OK");
        }
    }

    private async void BtnCheckIn_Clicked(object sender, EventArgs e)
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
                await DisplayAlert("Erro", "Não foi possível obter localização.", "OK");
                return;
            }

            var registro = new PontoRegistro
            {
                Horario = DateTime.UtcNow,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };

            var json = JsonSerializer.Serialize(registro);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/PontoRegistros", content);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Erro ao salvar na API.", "OK");
                return;
            }

            // 🔥 Adiciona ponto no mapa
            var point = SphericalMercator.FromLonLat(location.Longitude, location.Latitude);

            var position = new MPoint(point.x, point.y);

            var feature = new PointFeature(position);

            feature.Styles.Add(new SymbolStyle
            {
                SymbolScale = 1.0
            });

            var layer = new MemoryLayer
            {
                Features = new[] { feature }
            };

            mapa.Map.Layers.Add(layer);

            // 🔥 Centraliza no ponto
            mapa.Map.Navigator.CenterOn(position);
            mapa.Map.Navigator.ZoomTo(1000);

            await CarregarRegistros();

            await DisplayAlert("Sucesso", "Ponto registrado!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}