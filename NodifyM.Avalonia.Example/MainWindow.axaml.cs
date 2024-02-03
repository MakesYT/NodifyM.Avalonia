using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NodifyM.Avalonia.Example;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext=new MainWindowViewModel();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
    }
}