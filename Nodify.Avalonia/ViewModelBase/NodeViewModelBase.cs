using System.Collections.ObjectModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nodify.Avalonia.ViewModelBase;

public partial class NodeViewModelBase : ObservableObject
{
    [ObservableProperty] private Point _location;

    [ObservableProperty] private string _title;
    [ObservableProperty] private ObservableCollection<ConnectorViewModelBase> input = new();
    [ObservableProperty] private ObservableCollection<ConnectorViewModelBase> output = new();
}