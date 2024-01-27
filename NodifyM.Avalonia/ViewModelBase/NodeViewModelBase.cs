using System.Collections.ObjectModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Nodify.Avalonia.ViewModelBase;

public partial class NodeViewModelBase : ObservableObject
{
    [ObservableProperty] private Point _location;

    [ObservableProperty] private string _title;
    [ObservableProperty] private string _footer;
    [ObservableProperty] private ObservableCollection<ConnectorViewModelBase> input = new();
    [ObservableProperty] private ObservableCollection<ConnectorViewModelBase> output = new();
}