using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Nova.Application.UseCases;
using Nova.Infrastructure.Midi;
using Nova.Midi;
using Nova.Presentation.ViewModels;
using Serilog;
using System.IO;

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

        var logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NovaSystemManager",
            "logs");
        Directory.CreateDirectory(logDir);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(logDir, "NovaSystemManager-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        services.AddSingleton<ILogger>(Log.Logger);
        
        // Infrastructure
        services.AddSingleton<IMidiPort, DryWetMidiPort>();
        
        // Application
        services.AddTransient<IConnectUseCase, ConnectUseCase>();
        services.AddTransient<IDownloadBankUseCase, DownloadBankUseCase>();
        services.AddTransient<ISaveSystemDumpUseCase, SaveSystemDumpUseCase>();
        services.AddTransient<IRequestSystemDumpUseCase, RequestSystemDumpUseCase>();
        services.AddTransient<IRequestPresetUseCase, RequestPresetUseCase>();
        services.AddTransient<ISavePresetUseCase, SavePresetUseCase>();
        services.AddTransient<SaveBankUseCase>();
        services.AddTransient<LoadBankUseCase>();
        services.AddTransient<ExportPresetUseCase>();
        services.AddTransient<IExportPresetUseCase, ExportSyxPresetUseCase>();
        services.AddTransient<ImportPresetUseCase>();
        services.AddTransient<IGetCCMappingsUseCase, GetCCMappingsUseCase>();
        services.AddTransient<IUpdateCCMappingUseCase, UpdateCCMappingUseCase>();
        services.AddTransient<IGetProgramMapInUseCase, GetProgramMapInUseCase>();
        services.AddTransient<IUpdateProgramMapInUseCase, UpdateProgramMapInUseCase>();
        services.AddTransient<IGetProgramMapOutUseCase, GetProgramMapOutUseCase>();
        services.AddTransient<IUpdateProgramMapOutUseCase, UpdateProgramMapOutUseCase>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<ProgramMapInViewModel>();
        services.AddTransient<ProgramMapOutViewModel>();
        services.AddTransient<CCMappingViewModel>();
        
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };

            desktop.Exit += (_, _) => Log.CloseAndFlush();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
