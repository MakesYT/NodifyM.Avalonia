using Avalonia.Controls;

namespace NodifyM.Avalonia.Example;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext=new MainWindowViewModel();
    }
}