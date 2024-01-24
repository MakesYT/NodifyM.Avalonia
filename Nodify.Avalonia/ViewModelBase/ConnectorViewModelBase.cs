using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nodify.Avalonia.ViewModelBase;

public partial class ConnectorViewModelBase : ObservableObject
{
    [ObservableProperty]
    private Point _anchor;
    
    [ObservableProperty]
    private string _title;
    [ObservableProperty]
    private bool _isConnected;
}