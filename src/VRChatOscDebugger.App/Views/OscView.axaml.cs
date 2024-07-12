using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VRChatOscDebugger.App.ViewModels;

namespace VRChatOscDebugger.App.Views;

public partial class OscView : UserControl
{
    private readonly OscViewModel ViewModel;

    public OscView()
    {
        InitializeComponent();

        ViewModel = Program.ServiceProvider.GetRequiredService<OscViewModel>();
        DataContext = ViewModel;
    }

    private async void OscQueryService_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems[0] is not OscQueryServiceItemViewModel viewModel) return;

        if (viewModel.IsManual)
        {
            Program.ServiceProvider.GetRequiredService<ILogger<OscViewModel>>()
                .LogInformation("Enter manual IP and port");
            return;
        }

        await ViewModel.ConnectOsc(viewModel);
    }
}