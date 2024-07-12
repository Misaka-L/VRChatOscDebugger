using Avalonia;
using Avalonia.Controls;
using Material.Icons;

namespace VRChatOscDebugger.App.Controls;

public partial class RailNavigationItem : UserControl
{
    public static readonly StyledProperty<MaterialIconKind> IconProperty = AvaloniaProperty.Register<RailNavigationItem, MaterialIconKind>(
        "Icon");

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<RailNavigationItem, string>(
        "Title");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public MaterialIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public RailNavigationItem()
    {
        InitializeComponent();
        DataContext = this;
    }
}