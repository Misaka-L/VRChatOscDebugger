using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VRChatOscDebugger.App;

public static class HostExtensions
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static HostApplicationBuilder ConfigureAvaloniaAppBuilder(this HostApplicationBuilder hostBuilder, string[] args)
    {
        var applicationLifetime = new ClassicDesktopStyleApplicationLifetime
        {
            Args = args,
            ShutdownMode = ShutdownMode.OnMainWindowClose
        };

        var appBuilder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .SetupWithLifetime(applicationLifetime)
            .LogToTrace();

        hostBuilder.Services.AddSingleton(appBuilder);

        if (appBuilder.Instance == null)
        {
            appBuilder.SetupWithoutStarting();
        }

        hostBuilder.Services
            .AddSingleton(appBuilder.Instance!)
            .AddSingleton(applicationLifetime);

        return hostBuilder;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static async Task<int> RunAvaloniaAppAsync(this IHost host, string[] args, CancellationToken token = default)
    {
        var lifetime = host.Services.GetRequiredService<ClassicDesktopStyleApplicationLifetime>();

        host.Services.GetRequiredService<Application>();

        await host.StartAsync(token);

        var result = lifetime.Start(args);

        await host.StopAsync(token);

        await host.WaitForShutdownAsync(token);

        return result;
    }
}