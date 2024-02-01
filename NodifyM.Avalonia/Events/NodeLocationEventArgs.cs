using Avalonia;
using Avalonia.Interactivity;
using NodifyM.Avalonia.Controls;

namespace NodifyM.Avalonia.Events;
public delegate void NodeLocationEventHandler(object sender, NodeLocationEventArgs e);
public class NodeLocationEventArgs : RoutedEventArgs{
    public Point Location { get; set; }
    public BaseNode Sender{ get; set; }
    public bool Stop{ get; set; }
    public NodeLocationEventArgs(Point location, BaseNode sender,RoutedEvent routedEvent,bool stop = false)
    {
        Location = location;
        this.Sender = sender;
        this.RoutedEvent = routedEvent;
        this.Stop = stop;
    }
}
