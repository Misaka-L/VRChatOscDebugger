using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Rug.Osc;
using VRC.OSCQuery;
using VRChatOscDebugger.OscCore.Models;

namespace VRChatOscDebugger.OscCore.Services;

public class OscService(OSCQueryService oscQueryService, ILogger<OscService> logger)
{
    public IPEndPoint? ConnectedOscSendEndPoint { get; private set; }
    public IPEndPoint? ConnectedOscQueryEndPoint { get; private set; }

    public event EventHandler<OscMessage>? OnOscMessageReceived;
    public event EventHandler? OnOscConnected;

    public async Task<OscQueryNode?> ConnectByOscQueryAsync(string ipAddress, int port)
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{ipAddress}:{port}")
        };

        var hostInfo = await httpClient.GetFromJsonAsync<OscQueryHostInfo>("/?HOST_INFO");

        var nodes = await httpClient.GetFromJsonAsync<OscQueryNode>("/");

        oscQueryService.RefreshServices();

        ConnectedOscSendEndPoint = new IPEndPoint(IPAddress.Parse(hostInfo.OscIp), hostInfo.OscPort);
        ConnectedOscQueryEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

        OnOscConnected?.Invoke(this, EventArgs.Empty);

        return nodes;
    }

    internal void NotifyOscMessageReceive(OscMessage message)
    {
        if (!message.Origin.Address.Equals(ConnectedOscSendEndPoint.Address))
            return;

        OnOscMessageReceived?.Invoke(this, message);
    }
}
