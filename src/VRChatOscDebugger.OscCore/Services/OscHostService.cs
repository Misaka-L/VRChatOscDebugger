using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rug.Osc;
using VRC.OSCQuery;

namespace VRChatOscDebugger.OscCore.Services;

public class OscHostService(
    OSCQueryService oscQueryService,
    OscService oscService,
    OscReceiver oscReceiver,
    ILogger<OscHostService> logger) : IHostedService
{
    private Timer? _refreshTimer;
    private CancellationTokenSource _receiveCancellationTokenSource = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        oscQueryService.AddEndpoint<string>("/avatar/change", Attributes.AccessValues.WriteOnly);

        oscQueryService.AddEndpoint<string>("/metadata/version", Attributes.AccessValues.ReadOnly, ["snapshot"],
            "Version");

        oscQueryService.RefreshServices();

        _ = Task.Run(() =>
        {
            while (!_receiveCancellationTokenSource.Token.IsCancellationRequested && oscReceiver.State != OscSocketState.Closed)
            {
                if (oscReceiver.State != OscSocketState.Connected) continue;

                var packet = oscReceiver.Receive();

                logger.LogInformation("Received packet type {Type} from {Address}:{Port}: {@Message}",
                    packet.GetType().Name, packet.Origin.Address, packet.Origin.Port, packet.ToString());

                if (packet is OscMessage message)
                    oscService.NotifyOscMessageReceive(message);
            }
        }, _receiveCancellationTokenSource.Token);

        _refreshTimer = new Timer(_ => { oscQueryService.RefreshServices(); }, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _receiveCancellationTokenSource.CancelAsync();

        if (_refreshTimer != null)
            await _refreshTimer.DisposeAsync();
    }
}
