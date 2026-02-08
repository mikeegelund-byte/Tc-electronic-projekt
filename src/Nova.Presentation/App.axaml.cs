using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Nova.Application.Library;
using Nova.Application.UseCases;
using Nova.Infrastructure.Library;
using Nova.Infrastructure.Midi;
using Nova.Midi;
using Nova.Presentation.ViewModels;
using System.IO;
using Serilog;

namespace Nova.Presentation;

public partial class App : global::Avalonia.Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nova.log");

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(logPath,
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        logger.Information("Nova app starting. Log path: {LogPath}", logPath);

        Log.Logger = logger;
        services.AddSingleton<ILogger>(logger);
        
        // Infrastructure
        services.AddSingleton<IMidiPort, DryWetMidiPort>();
        services.AddSingleton<ILibraryRepository, FileSystemLibraryRepository>();
        
        // Application
        services.AddTransient<IConnectUseCase, ConnectUseCase>();
        services.AddTransient<IDownloadBankUseCase, DownloadBankUseCase>();
        services.AddTransient<IRequestPresetUseCase, RequestPresetUseCase>();
        services.AddTransient<IRequestSystemDumpUseCase, RequestSystemDumpUseCase>();
        services.AddTransient<ISaveSystemDumpUseCase, SaveSystemDumpUseCase>();
        services.AddTransient<ISavePresetUseCase, SavePresetUseCase>();
        services.AddTransient<ISaveBankUseCase, SaveBankUseCase>();
        services.AddTransient<ILoadBankUseCase, LoadBankUseCase>();
        services.AddTransient<IExportPresetUseCase, ExportSyxPresetUseCase>();
        services.AddTransient<IImportPresetUseCase, ImportPresetUseCase>();
        services.AddTransient<ISendBankToHardwareUseCase, SendBankToHardwareUseCase>();
        services.AddTransient<IGetCCMappingsUseCase, GetCCMappingsUseCase>();
        services.AddTransient<IUpdateCCMappingUseCase, UpdateCCMappingUseCase>();
        services.AddTransient<IGetProgramMapInUseCase, GetProgramMapInUseCase>();
        services.AddTransient<IUpdateProgramMapInUseCase, UpdateProgramMapInUseCase>();
        services.AddTransient<IGetProgramMapOutUseCase, GetProgramMapOutUseCase>();
        services.AddTransient<IUpdateProgramMapOutUseCase, UpdateProgramMapOutUseCase>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<PresetDetailViewModel>();
        services.AddTransient<CCMappingViewModel>();
        services.AddTransient<ProgramMapInViewModel>();
        services.AddTransient<ProgramMapOutViewModel>();
        
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
