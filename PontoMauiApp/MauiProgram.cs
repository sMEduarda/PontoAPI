using Microsoft.Extensions.Logging;

namespace PontoMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // ✅ HttpClient (HTTPS + emulador Android)
        builder.Services.AddSingleton<HttpClient>(sp =>
            new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            })
            {
                BaseAddress = new Uri("https://10.0.2.2:7001/")
            });

        // ✅ REGISTRAR PÁGINAS
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<AppShell>(); // 🔥 FALTAVA ISSO

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}