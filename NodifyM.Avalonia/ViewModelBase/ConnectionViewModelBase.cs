using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class ConnectionViewModelBase: ObservableObject
{
    [ObservableProperty] public ConnectorViewModelBase source;
    [ObservableProperty] public ConnectorViewModelBase target;
    
    public ConnectionViewModelBase(ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        Source = source;
        Target = target;
    }
}