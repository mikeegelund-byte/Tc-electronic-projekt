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

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger = logger;
        services.AddSingleton<ILogger>(logger);
        
        // Infrastructure
        services.AddSingleton<IMidiPort, DryWetMidiPort>();
        services.AddSingleton<ILibraryRepository, FileSystemLibraryRepository>();
        
        // Application
        services.AddTransient<IConnectUseCase, ConnectUseCase>();
        services.AddTransient<IDownloadBankUseCase, DownloadBankUseCase>();
        services.AddTransient<IRequestPresetUseCase, RequestPresetUseCase>();
        services.AddTransient<ISaveSystemDumpUseCase, SaveSystemDumpUseCase>();
        services.AddTransient<ISavePresetUseCase, SavePresetUseCase>();
        services.AddTransient<ISaveBankUseCase, SaveBankUseCase>();
        services.AddTransient<ILoadBankUseCase, LoadBankUseCase>();
        services.AddTransient<IExportPresetUseCase, ExportSyxPresetUseCase>();
        services.AddTransient<IImportPresetUseCase, ImportPresetUseCase>();
        services.AddTransient<ISendBankToHardwareUseCase, SendBankToHardwareUseCase>();
        services.AddTransient<IGetCCMappingsUseCase, GetCCMappingsUseCase>();
        services.AddTransient<IUpdateCCMappingUseCase, UpdateCCMappingUseCase>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<CCMappingViewModel>();
        
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
