using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Nodify.Avalonia.ViewModelBase;

public partial class NodifyEditorViewModelBase : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<NodeViewModelBase> nodes = new();
    [ObservableProperty]
    private ObservableCollection<ConnectionViewModelBase> connections=new ();
    [RelayCommand]
    private void DisconnectConnector(ConnectorViewModelBase connector)
    {
        var connections = Connections.Where(e => e.Source == connector || e.Target == connector).ToList();
        for (var i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            Connections.Remove(connection);
            if (Connections.All(e => e.Source != connection.Source))
            {
                connection.Source.IsConnected = false;
            }

            if (Connections.All(e => e.Target != connection.Target))
            {
                connection.Target.IsConnected = false;
            }

            
        }
        
    }
    public void Connect(ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        
        Connections.Add(new ConnectionViewModelBase(source, target));
    }

    
}