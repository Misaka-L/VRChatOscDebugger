using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rug.Osc;
using VRC.OSCQuery;
using VRChatOscDebugger.Core.Utils;
using VRChatOscDebugger.OscCore.Services;

namespace VRChatOscDebugger.OscCore;

public static class HostExtensions
{
    public static HostApplicationBuilder AddOscServices(this HostApplicationBuilder hostBuilder)
    {
        var udpPort = Extensions.GetAvailableUdpPort();
        var tcpPort = Extensions.GetAvailableTcpPort();

        hostBuilder.Services.AddSingleton<OscService>();
        hostBuilder.Services.AddSingleton<OscReceiver>(_ =>
        {
            var oscReceiver = new OscReceiver(udpPort);

            oscReceiver.Connect();

            return oscReceiver;
        });
        hostBuilder.Services.AddSingleton<OSCQueryService>(services =>
        {
            var logger = services.GetRequiredService<ILogger<OSCQueryService>>();
            var serviceName =
                $"VRCOscDebugger-{Guid.NewGuid().ToString("D")[..4]}";

            var osuQueryService = new OSCQueryServiceBuilder()
                .WithServiceName(serviceName)
                .WithHostIPs(NetworkUtils.GetLocalIpAddressNonLoopback())
                .WithOscIPs(NetworkUtils.GetLocalIpAddressNonLoopback())
                .WithDynamicOscIp()
                .WithTcpPort(tcpPort)
                .WithUdpPort(udpPort)
                .WithLogger(logger)
                .WithDiscovery(new MeaModDiscovery(logger))
                .WithListenAnyHost()
                .AddHttpServer(services.GetRequiredService<ILoggerFactory>())
                .AdvertiseOSC()
                .AdvertiseOSCQuery()
                .Build();

            _ = Task.Run(async () =>
            {
                await osuQueryService.StartHttpServer();

                osuQueryService.RefreshServices();

                logger.LogInformation("Starting OSC service on {}",
                    osuQueryService.OscIP.Select(oscIp => $"{oscIp}:{osuQueryService.OscPort}"));
                logger.LogInformation("Starting OSCQuery service on {}",
                    osuQueryService.HostIP.Select(hostIp => $"http://{hostIp}:{osuQueryService.TcpPort}"));
                logger.LogInformation("With Service Name: {Name}", serviceName);
            });

            osuQueryService.OnOscServiceAdded += sender =>
            {
                logger.LogInformation("OSC Service Added: {ServiceType} {ServiceName} at {AddressList}",
                    sender.ServiceType, sender.Name,
                    sender.Addresses.Select(address => $"{address}:{sender.Port}"));
            };

            osuQueryService.OnOscQueryServiceAdded += profile =>
            {
                logger.LogInformation("OSCQuery Service Added: {ServiceType} {ServiceName} at {AddressList}",
                    profile.ServiceType, profile.Name,
                    profile.Addresses.Select(address => $"http://{address}:{profile.Port}"));
            };

            return osuQueryService;
        });

        hostBuilder.Services.AddHostedService<OscHostService>();

        return hostBuilder;
    }
}
