using Microsoft.Extensions.DependencyInjection;

namespace PontoMauiApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = Handler.MauiContext.Services.GetService<AppShell>();
            return new Window(shell);
        }
    }
}