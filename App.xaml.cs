using System.Configuration;
using System.Data;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using WPF_Tool_MultiFolderCreator.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommunityToolkit.Mvvm.Messaging;
using WPF_Tool_MultiFolderCreator.ViewModels;
using WPF_Tool_MultiFolderCreator.Services.Logging;

namespace WPF_Tool_MultiFolderCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        private readonly IHost _host;

        // Statische Property für Service-Zugriff hinzufügen
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();

            // Services verfügbar machen
            Services = _host.Services;
        }

        // Services in separate Methode auslagern für bessere Übersichtlichkeit
        private void ConfigureServices(IServiceCollection services)
        {
            // Messenger
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

            // Services
            services.AddSingleton<LoggingService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<NameCorrectionViewModel>();

            // Views
            services.AddTransient<NameCorrectionDialog>();  // Als Transient, da mehrfach verwendet
            services.AddSingleton<MainWindow>();           // Singleton, da nur einmal benötigt
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}",
                              "Fehler",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_host != null)
                {
                    await _host.StopAsync();
                    _host.Dispose();
                }

                base.OnExit(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Beenden der Anwendung: {ex.Message}");
            }
        }
    }
}
