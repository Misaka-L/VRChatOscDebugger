using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using VRChatOscDebugger.App.ViewModels;

namespace VRChatOscDebugger.App.Controls;

public partial class AppStatusBar : UserControl
{
    private readonly OscStatusBarViewModel _viewModel;

    public AppStatusBar()
    {
        _viewModel = Program.ServiceProvider.GetRequiredService<OscStatusBarViewModel>();
        DataContext = _viewModel;

        InitializeComponent();
    }
}
