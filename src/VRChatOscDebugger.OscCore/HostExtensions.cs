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
                .WithHostIP(IPAddress.Loopback)
                .WithOscIP(NetworkUtils.GetLocalIpAddressNonLoopback().FirstOrDefault())
                .WithTcpPort(tcpPort)
                .WithUdpPort(udpPort)
                .WithLogger(logger)
                .WithDiscovery(new MeaModDiscovery(logger))
                .StartHttpServer()
                .AdvertiseOSC()
                .AdvertiseOSCQuery()
                .Build();

            osuQueryService.RefreshServices();

            logger.LogInformation("Starting OSC service on {OscIp}:{OscPort}", osuQueryService.OscIP,
                osuQueryService.OscPort);
            logger.LogInformation("Starting OSCQuery service on http://{OscIp}:{OscPort}", osuQueryService.HostIP,
                osuQueryService.TcpPort);
            logger.LogInformation("With Service Name: {Name}", serviceName);

            osuQueryService.OnOscServiceAdded += sender =>
            {
                logger.LogInformation("OSC Service Added: {ServiceType} {ServiceName} at {Address}:{Port}",
                    sender.serviceType, sender.name, sender.address, sender.port);
            };

            osuQueryService.OnOscQueryServiceAdded += profile =>
            {
                logger.LogInformation("OSCQuery Service Added: {ServiceType} {ServiceName} at {Address}:{Port}",
                    profile.serviceType, profile.name, profile.address, profile.port);
            };

            return osuQueryService;
        });

        hostBuilder.Services.AddHostedService<OscHostService>();

        return hostBuilder;
    }
}
