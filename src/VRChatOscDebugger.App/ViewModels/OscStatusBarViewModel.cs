using CommunityToolkit.Mvvm.ComponentModel;
using VRC.OSCQuery;
using VRChatOscDebugger.OscCore.Services;

namespace VRChatOscDebugger.App.ViewModels;

public partial class OscStatusBarViewModel : ViewModelBase
{
    [ObservableProperty] private string _oscQueryServiceName = "";
    [ObservableProperty] private int _oscQueryPort = -1;

    [ObservableProperty] private int _oscPort = -1;

    [ObservableProperty] private string _connectedOscIp = "";
    [ObservableProperty] private int _connectedOscPort = -1;

    [ObservableProperty] private string _connectedOscQueryIp = "";
    [ObservableProperty] private int _connectedOscQueryPort = -1;

    private readonly OscService _oscService;
    private readonly OSCQueryService _oscQueryService;

    public OscStatusBarViewModel(OscService oscService, OSCQueryService oscQueryService)
    {
        _oscService = oscService;
        _oscQueryService = oscQueryService;

        UpdateStatus();
        _oscService.OnOscConnected += (_, _) => UpdateStatus();
    }

    private void UpdateStatus()
    {
        OscQueryServiceName = _oscQueryService.ServerName;
        OscQueryPort = _oscQueryService.TcpPort;

        OscPort = _oscQueryService.OscPort;

        ConnectedOscIp = _oscService.ConnectedOscSendEndPoint?.Address.ToString() ?? "No connected";
        ConnectedOscPort = _oscService.ConnectedOscSendEndPoint?.Port ?? 0;

        ConnectedOscQueryIp = _oscService.ConnectedOscQueryEndPoint?.Address.ToString() ?? "No connected";
        ConnectedOscQueryPort = _oscService.ConnectedOscQueryEndPoint?.Port ?? 0;
    }
}
