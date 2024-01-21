using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Nodify.Avalonia.Helpers;
using Tmds.DBus.Protocol;

namespace Nodify.Avalonia.Controls;

public class Connector : ContentControl
{
    protected const string ElementConnector = "PART_Connector";

    #region Routed Events

    public static readonly RoutedEvent PendingConnectionStartedEvent = RoutedEvent.Register<Connector,PendingConnectionEventArgs>(nameof(PendingConnectionStarted),RoutingStrategies.Bubble);
    public static readonly RoutedEvent PendingConnectionCompletedEvent = RoutedEvent.Register<Connector,PendingConnectionEventArgs>(nameof(PendingConnectionCompleted),RoutingStrategies.Bubble);
    public static readonly RoutedEvent PendingConnectionDragEvent = RoutedEvent.Register<Connector,PendingConnectionEventArgs>(nameof(PendingConnectionDrag),RoutingStrategies.Bubble);
    public static readonly RoutedEvent DisconnectEvent = RoutedEvent.Register<Connector,PendingConnectionEventArgs>(nameof(Disconnect),RoutingStrategies.Bubble);
        

    /// <summary>Triggered by the <see cref="Connector.Connect"/> gesture.</summary>
    public event PendingConnectionEventHandler PendingConnectionStarted
    {
        add => AddHandler(PendingConnectionStartedEvent, value);
        remove => RemoveHandler(PendingConnectionStartedEvent, value);
    }

    /// <summary>Triggered by the <see cref="Connector.Connect"/> gesture.</summary>
    public event PendingConnectionEventHandler PendingConnectionCompleted
    {
        add => AddHandler(PendingConnectionCompletedEvent, value);
        remove => RemoveHandler(PendingConnectionCompletedEvent, value);
    }

    /// <summary>
    /// Occurs when the mouse is changing position and the <see cref="Connector"/> has mouse capture.
    /// </summary>
    public event PendingConnectionEventHandler PendingConnectionDrag
    {
        add => AddHandler(PendingConnectionDragEvent, value);
        remove => RemoveHandler(PendingConnectionDragEvent, value);
    }

    /// <summary>Triggered by the <see cref="Connector.Disconnect"/> gesture.</summary>
    public event ConnectorEventHandler Disconnect
    {
        add => AddHandler(DisconnectEvent, value);
        remove => RemoveHandler(DisconnectEvent, value);
    }

    #endregion

    #region Dependency Properties

       
    public static readonly AvaloniaProperty<Point> AnchorProperty = AvaloniaProperty.Register<Connector, Point>(nameof(Anchor), BoxValue.Point);
    public static readonly AvaloniaProperty<bool> IsConnectedProperty = AvaloniaProperty.Register<Connector, bool>(nameof(Anchor), BoxValue.False);
    public static readonly AvaloniaProperty<ICommand> DisconnectCommandProperty = AvaloniaProperty.Register<Connector, ICommand>(nameof(DisconnectCommand));
    public static readonly AvaloniaProperty<bool> IsPendingConnectionProperty = AvaloniaProperty.Register<Connector, bool>(nameof(IsPendingConnection), BoxValue.False);
    

    /// <summary>
    /// Gets the location where <see cref="Connection"/>s can be attached to. 
    /// Bind with <see cref="BindingMode.OneWayToSource"/>
    /// </summary>
    public Point Anchor
    {
        get => (Point)GetValue(AnchorProperty);
        set => SetValue(AnchorProperty, value);
    }

    /// <summary>
    /// If this is set to false, the <see cref="Disconnect"/> event will not be invoked and the connector will stop updating its <see cref="Anchor"/> when moved, resized etc.
    /// </summary>
    public bool IsConnected
    {
        get => (bool)GetValue(IsConnectedProperty);
        set => SetValue(IsConnectedProperty, value);
    }

    /// <summary>
    /// Gets a value that indicates whether a <see cref="PendingConnection"/> is in progress for this <see cref="Connector"/>.
    /// </summary>
    public bool IsPendingConnection
    {
        get => (bool)GetValue(IsPendingConnectionProperty);
        protected set => SetValue(IsPendingConnectionProperty, value);
    }

    /// <summary>
    /// Invoked if the <see cref="Disconnect"/> event is not handled.
    /// Parameter is the <see cref="FrameworkElement.DataContext"/> of this control.
    /// </summary>
    public ICommand? DisconnectCommand
    {
        get => (ICommand?)GetValue(DisconnectCommandProperty);
        set => SetValue(DisconnectCommandProperty, value);
    }

    #endregion
    
    
    static Connector()
    {
        //DefaultStyleKeyProperty.OverrideMetadata(typeof(Connector), new FrameworkPropertyMetadata(typeof(Connector)));
        FocusableProperty.OverrideMetadata(typeof(Connector), new StyledPropertyMetadata<bool>(BoxValue.True));
    }
    
    

}