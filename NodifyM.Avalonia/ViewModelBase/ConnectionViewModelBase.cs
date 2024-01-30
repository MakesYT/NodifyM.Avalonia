
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class ConnectionViewModelBase: ObservableObject
{
    [ObservableProperty] public ConnectorViewModelBase source;
    [ObservableProperty] public ConnectorViewModelBase target;
    [ObservableProperty] public string text;

    private NodifyEditorViewModelBase nodifyEditor
    {
        get;
    }
    
    public ConnectionViewModelBase(NodifyEditorViewModelBase nodifyEditor,ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        this.nodifyEditor = nodifyEditor;
        Source = source;
        Target = target;
    }
    public ConnectionViewModelBase(NodifyEditorViewModelBase nodifyEditor,ConnectorViewModelBase source, ConnectorViewModelBase target, string text)
    {
        this.nodifyEditor = nodifyEditor;
        Source = source;
        Target = target;
        Text = text;
    }
    [RelayCommand]
    public virtual void DisconnectConnection(ConnectionViewModelBase connection)
    {
        nodifyEditor.Connections.Remove(connection);
        if (Enumerable.All<ConnectionViewModelBase>(nodifyEditor.Connections, e => (e.Source) != connection.Source&&(e.Target) != connection.Source))
        {
            connection.Source.IsConnected = false;
        }

        if (Enumerable.All<ConnectionViewModelBase>(nodifyEditor.Connections, e => (e.Target != connection.Target)&&(e.Source != connection.Target)))
        {
            connection.Target.IsConnected = false;
        }
        
    }
    [RelayCommand]
    public virtual void SplitConnection(Point location)
    {
        var knot = new KnotNodeViewModel
        {
            Location = location,
            Connector = new ConnectorViewModelBase()
        };
        nodifyEditor.Nodes.Add(knot);

        nodifyEditor.Connect(Source, knot.Connector);
        nodifyEditor.Connect(knot.Connector, this.Target);

        nodifyEditor.Connections.Remove(this);
    }
}