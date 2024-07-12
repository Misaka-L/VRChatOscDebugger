using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using VRC.OSCQuery;
using VRChatOscDebugger.OscCore.Services;

namespace VRChatOscDebugger.App.ViewModels;

public partial class OscQueryNodeViewModel : ViewModelBase
{
    private OscService _oscService;

    public OscQueryNodeViewModel()
    {
        _oscService = Program.ServiceProvider.GetRequiredService<OscService>();

        _oscService.OnOscMessageReceived += (sender, message) =>
        {
            if (message.Address != FullPath)
                return;

            Dispatcher.UIThread.Invoke(() =>
            {
                Value = string.Join(',', message.Select(arg => arg.ToString()).ToArray());
            });
        };
    }

    public string? Description { get; set; }
    public string FullPath { get; set; } = "/";
    public Attributes.AccessValues? Access { get; set; }
    public string Type { get; set; }

    [ObservableProperty] private object? _value;

    public OscQueryNodeViewModel[] Childrens { get; set; } = [];
}
