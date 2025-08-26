using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Windows;
using WinBaseSoftwareInstall.Interfaces;
using WinBaseSoftwareInstall.Services;
using WinBaseSoftwareInstall.ViewModels;
using WinBaseSoftwareInstall.Views;

namespace WinBaseSoftwareInstall
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigureSerilog();

            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            MainWindowView mainWindow = _serviceProvider.GetRequiredService<MainWindowView>();
            mainWindow.Show();
        }

        private void ConfigureSerilog()
        {
            string logPath = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Logs",
                            "app.log"
                        );

            string logDirectory = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7, // Keep logs for 7 days
                    fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB per file
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ThreadId}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Information("Application starting up...");
            Log.Information("Log files will be saved to: {LogPath}", logPath);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configure Logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders(); // Remove default providers
                builder.AddSerilog(dispose: true); // Add Serilog
            });

            // Register Services
            services.AddSingleton<IUserService, UserService>();

            // Register ViewModels
            services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();

            // Register Views
            services.AddSingleton<MainWindowView>();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Log.Information("Application shutting down...");

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}