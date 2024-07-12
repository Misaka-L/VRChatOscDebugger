using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRC.OSCQuery;
using VRChatOscDebugger.OscCore.Models;
using VRChatOscDebugger.OscCore.Services;

namespace VRChatOscDebugger.App.ViewModels;

public partial class OscViewModel : ViewModelBase
{
    private readonly OscService _oscService;

    [ObservableProperty] private bool _isConnected;

    [ObservableProperty] private ObservableCollection<OscQueryServiceItemViewModel> _oscQueryServices = [];
    [ObservableProperty] private HierarchicalTreeDataGridSource<OscQueryNodeViewModel> _oscParametersView = new([])
    {
        Columns =
        {
            new HierarchicalExpanderColumn<OscQueryNodeViewModel>(
                new TextColumn<OscQueryNodeViewModel, string>("FullPath", model => model.FullPath),
                model => model.Childrens
            ),
            new TextColumn<OscQueryNodeViewModel, string>("Type", model => model.Type),
            new TextColumn<OscQueryNodeViewModel, string>("Value", model => model.Value.ToString() ?? ""),
            // new TextColumn<OscQueryNodeViewModel, string>("Access",
            //     model => model.Access != null ? model.Access.ToString() : "NoAccess"),
            new TextColumn<OscQueryNodeViewModel, string>("Description", model => model.Description ?? "")
        }
    };

    public OscViewModel(OSCQueryService oscQueryService, OscService oscService)
    {
        _oscService = oscService;

        OscQueryServices.Add(new OscQueryServiceItemViewModel
        {
            IsManual = true,
            Name = "Enter IP and port manually",
        });

        oscService.OnOscMessageReceived += (_, message) =>
        {
            if (message.Address == "/avatar/change")
            {
                Dispatcher.UIThread.Invoke(async () =>
                {
                    await ConnectOscInternal(oscService.ConnectedOscQueryEndPoint.Address.ToString(),
                        oscService.ConnectedOscQueryEndPoint.Port);
                });
            }
        };

        oscQueryService.OnOscQueryServiceAdded += profile =>
        {
            if (OscQueryServices.Any(p => p.Name == profile.name))
            {
                return;
            }

            OscQueryServices.Add(new OscQueryServiceItemViewModel
            {
                Name = profile.name,
                Address = profile.address.ToString(),
                Port = profile.port,
                Type = profile.serviceType.ToString()
            });
        };
    }

    [RelayCommand]
    public async Task ConnectOsc(OscQueryServiceItemViewModel oscQueryServiceItemViewModel)
    {
        await ConnectOscInternal(oscQueryServiceItemViewModel.Address, oscQueryServiceItemViewModel.Port);
    }

    private async Task ConnectOscInternal(string address, int port)
    {
        var parameters = await _oscService.ConnectByOscQueryAsync(address, port);

        var rootModel = ToViewModel(parameters);

        OscParametersView.Items = rootModel.Childrens;

        IsConnected = true;

        OscQueryNodeViewModel ToViewModel(OscQueryNode node)
        {
            return new OscQueryNodeViewModel
            {
                Access = node.Access,
                Description = node.Description,
                FullPath = node.FullPath,
                Type = node.Type,
                Value = (node.Value ?? []).Length >= 1 ? node.Value.First() : null,
                Childrens = node.Contents.Select(node => ToViewModel(node.Value)).ToArray()
            };
        }
    }
}
