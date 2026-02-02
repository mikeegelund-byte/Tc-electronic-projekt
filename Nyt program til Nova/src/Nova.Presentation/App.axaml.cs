using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Nova.Application.UseCases;
using Nova.Infrastructure.Midi;
using Nova.Midi;
using Nova.Presentation.ViewModels;

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
        
        // Infrastructure
        services.AddSingleton<IMidiPort, DryWetMidiPort>();
        
        // Application
        services.AddTransient<IConnectUseCase, ConnectUseCase>();
        services.AddTransient<IDownloadBankUseCase, DownloadBankUseCase>();
        services.AddTransient<ISaveSystemDumpUseCase, SaveSystemDumpUseCase>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        
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