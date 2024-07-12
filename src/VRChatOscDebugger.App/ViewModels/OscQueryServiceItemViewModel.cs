using CommunityToolkit.Mvvm.ComponentModel;

namespace VRChatOscDebugger.App.ViewModels;

public partial class OscQueryServiceItemViewModel : ObservableObject
{
    [ObservableProperty] private string _address;
    [ObservableProperty] private int _port;
    [ObservableProperty] private string _type;
    [ObservableProperty] private string _name;
    [ObservableProperty] private bool _isManual;
}