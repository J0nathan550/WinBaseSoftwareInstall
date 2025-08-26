using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;
using WinBaseSoftwareInstall.Interfaces;
using WinBaseSoftwareInstall.Services;
using WinBaseSoftwareInstall.ViewModels;
using WinBaseSoftwareInstall.Views;

namespace WinBaseSoftwareInstall;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        ConfigureSerilog();

        SetupExceptionHandling();

        ServiceCollection serviceCollection = new();
        ConfigureServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        MainWindowView mainWindow = _serviceProvider.GetRequiredService<MainWindowView>();
        mainWindow.Show();
    }

    private static void ConfigureSerilog()
    {
        string logPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Logs",
                        "app.log"
                    );

        string logDirectory = Path.GetDirectoryName(logPath)!;
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
#if DEBUG
            .WriteTo.Debug(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
#endif
            .CreateLogger();

        Log.Information("Application starting up...");
        Log.Information("Log files will be saved to: {LogPath}", logPath);
    }

    private void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                   LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };
    }

    private static void LogUnhandledException(Exception exception, string source)
    {
        string message = $"Unhandled exception ({source})";
        try
        {
            System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception in LogUnhandledException");
        }
        finally
        {
            Log.Error(exception, message);
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configure Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders(); // Remove default providers
            builder.AddSerilog(dispose: true); // Add Serilog
#if DEBUG
            builder.AddDebug();
#endif
        });

        // Register Services
        services.AddSingleton<IUserService, UserService>();

        // Register ViewModels
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();

        // Register Views
        services.AddSingleton<MainWindowView>();
    }

    protected override void OnExit(ExitEventArgs e)
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