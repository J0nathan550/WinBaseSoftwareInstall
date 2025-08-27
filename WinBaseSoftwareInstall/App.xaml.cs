using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;
using WinBaseSoftwareInstall.Interfaces;
// using WinBaseSoftwareInstall.Services;
using WinBaseSoftwareInstall.ViewModels;
using WinBaseSoftwareInstall.Views;
#if !DEBUG
    using System.Diagnostics;
    using MessageBox = HandyControl.Controls.MessageBox;
#endif

namespace WinBaseSoftwareInstall;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    private string _logsPath = string.Empty;
    private MainWindowView? _mainWindowView;
#if !DEBUG
    private const string GITHUB_REPOSITORY = "https://github.com/J0nathan550/WinBaseSoftwareInstall";
#endif

    protected override void OnStartup(StartupEventArgs e)
    {
        ConfigureSerilog();

#if !DEBUG
        SetupExceptionHandling();
#endif

        ServiceCollection serviceCollection = new();
        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();

        _mainWindowView = ServiceProvider.GetRequiredService<MainWindowView>();
        _mainWindowView.Show();
    }

    private void ConfigureSerilog()
    {
        _logsPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Logs",
                        "app.log"
                    );

        string logDirectory = Path.GetDirectoryName(_logsPath)!;
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
                _logsPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7, // Keep logs for 7 days
                fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB per file
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ThreadId}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
#if !DEBUG
            .WriteTo.Debug(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
#endif
            .CreateLogger();

        Log.Information("Application starting up...");
        Log.Information("Log files will be saved to: {LogPath}", _logsPath);
    }

#if !DEBUG
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

    private void LogUnhandledException(Exception exception, string source)
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

        ShowFatalCrashDialog();
    }

    private void ShowFatalCrashDialog()
    {
        Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        try
        {
            _mainWindowView?.Close();

            MessageBoxResult result = MessageBox.Show($"There was unhandled error in the program. If issue keep coming back send report to GitHub!\n\n\nPress `Yes` to open GitHub repository, after create issues to report the bug.\n\n\nAfter pressing any of the buttons program will shutdown. Check Logs for full information.\n\n\nLogs Output: {_logsPath}", "Fatal Crash!", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = GITHUB_REPOSITORY,
                        UseShellExecute = true
                    });
                }
                catch (Exception e)
                {
                    Log.Error($"Couldn't open browser link: {e}");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error in crash dialog: {ex}");
        }
        finally
        {
            Current.Shutdown();
        }
    }
#endif

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configure Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders(); // Remove default providers
            builder.AddSerilog(dispose: true); // Add Serilog
        });

        // Register Services
        // services.AddSingleton<IUserService, UserService>();

        // Register ViewModels
        services.AddSingleton<ITitleBarNonClientViewModel, TitleBarNonClientViewModel>();
        services.AddTransient<IConfigDialogViewModel, ConfigDialogViewModel>();
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();

        // Register Views
        services.AddSingleton<TitleBarNonClientView>();
        services.AddTransient<ConfigDialogView>();
        services.AddSingleton<MainWindowView>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Application shutting down...");

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Log.CloseAndFlush();

        base.OnExit(e);
    }
}