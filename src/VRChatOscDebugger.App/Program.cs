using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VRChatOscDebugger.App.Controls;
using VRChatOscDebugger.App.ViewModels;
using VRChatOscDebugger.App.Views;
using VRChatOscDebugger.OscCore;

namespace VRChatOscDebugger.App;

sealed class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    [STAThread]
    private static async Task<int> Main(string[] args)
    {
        var hostBuilder = Host.CreateApplicationBuilder(args)
            .ConfigureAvaloniaAppBuilder(args);

        hostBuilder.AddOscServices();

        hostBuilder.Services.AddTransient<MainWindowViewModel>();
        hostBuilder.Services.AddTransient<MainWindow>();

        hostBuilder.Services.AddTransient<OscView>();
        hostBuilder.Services.AddTransient<OscViewModel>();

        hostBuilder.Services.AddTransient<OscConnection>();

        var host = hostBuilder.Build();

        ServiceProvider = host.Services;

        var lifetime = host.Services.GetRequiredService<ClassicDesktopStyleApplicationLifetime>();
        lifetime.MainWindow = host.Services.GetRequiredService<MainWindow>();

        return await host.RunAvaloniaAppAsync(args);
    }
}
