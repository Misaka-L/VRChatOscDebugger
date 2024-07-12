using System.Net;
using System.Net.Sockets;

namespace VRChatOscDebugger.Core.Utils;

public static class NetworkUtils
{
    public static IPAddress[] GetLocalIpAddressNonLoopback()
    {
        var hostName = Dns.GetHostName();

        return Dns.GetHostEntry(hostName).AddressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .ToArray();
    }
}
