using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class NodifyEditorViewModelBase : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<NodeViewModelBase> nodes = new();
    [ObservableProperty]
    private ObservableCollection<ConnectionViewModelBase> connections=new ();

    public PendingConnectionViewModelBase PendingConnection { get; set; }

    public NodifyEditorViewModelBase()
    {
        PendingConnection = new PendingConnectionViewModelBase(this);
    }
    [RelayCommand]
    private void DisconnectConnector(ConnectorViewModelBase connector)
    {
        var connections = Enumerable.Where<ConnectionViewModelBase>(Connections, e => e.Source == connector || e.Target == connector).ToList();
        for (var i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            Connections.Remove(connection);
            if (Enumerable.All<ConnectionViewModelBase>(Connections, e => e.Source != connection.Source))
            {
                connection.Source.IsConnected = false;
            }

            if (Enumerable.All<ConnectionViewModelBase>(Connections, e => e.Target != connection.Target))
            {
                connection.Target.IsConnected = false;
            }

            
        }
        
    }
    public void Connect(ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        if (source.Flow == ConnectorViewModelBase.ConnectorFlow.Output)
        {
            Connections.Add(new ConnectionViewModelBase(source, target));
        }else if (source.Flow == ConnectorViewModelBase.ConnectorFlow.Input)
        {
            Connections.Add(new ConnectionViewModelBase(target, source));
        }
        
        source.IsConnected = true;
        target.IsConnected = true;
    }

    
}