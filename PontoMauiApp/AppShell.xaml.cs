namespace PontoMauiApp
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage)
        {
            InitializeComponent();

            Items.Add(new ShellContent
            {
                Content = mainPage,
                Title = "Registro de Ponto"
            });
        }
    }
}
