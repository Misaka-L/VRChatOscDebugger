using Avalonia.Controls;
using VRChatOscDebugger.App.ViewModels;

namespace VRChatOscDebugger.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        this.DataContext = viewModel;
    }
}