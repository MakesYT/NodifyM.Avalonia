using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Nodify.Avalonia.Helpers;
using Nodify.Avalonia.ViewModelBase;
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

       
    public static readonly StyledProperty<Point> AnchorProperty = AvaloniaProperty.Register<Connector, Point>(nameof(Anchor), BoxValue.Point);
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
    protected internal NodifyEditor? Editor { get; private set; }
    protected Control? Thumb { get; private set; }
    protected Node? Container { get; private set; }
    public static bool AllowPendingConnectionCancellation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the connection should be completed in two steps.
    /// </summary>
    public static bool EnableStickyConnections { get; set; }

    /// <summary>
    /// Gets the <see cref="NodifyEditor"/> that owns this <see cref="Container"/>.
    /// </summary>
    private Point _lastUpdatedContainerPosition;
    private Point _thumbCenter;
    static Connector()
    {
        //DefaultStyleKeyProperty.OverrideMetadata(typeof(Connector), new FrameworkPropertyMetadata(typeof(Connector)));
        FocusableProperty.OverrideMetadata(typeof(Connector), new StyledPropertyMetadata<bool>(BoxValue.True));
    }
    private void OnConnectorLoaded(object sender, RoutedEventArgs? e)
        => TrySetAnchorUpdateEvents(true);

    private void OnConnectorUnloaded(object sender, RoutedEventArgs e)
        => TrySetAnchorUpdateEvents(false);
    private void TrySetAnchorUpdateEvents(bool value)
    {
        if (Container != null && Editor != null)
        {
            // If events are not already hooked and we are asked to subscribe
            if (value)
            {
                Container.LocationChanged += OnLocationChanged;
                Container.SizeChanged += OnContainerSizeChanged;
            }
            // If events are already hooked and we are asked to unsubscribe
            else if ( !value)
            {
                Container.LocationChanged -= OnLocationChanged;
                Container.SizeChanged -= OnContainerSizeChanged;
            }
        }
    }
    private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e)
        => UpdateAnchor(Container!.Location);
    private void OnLocationChanged(object sender, RoutedEventArgs e)
        => UpdateAnchor(Container!.Location);
    public void UpdateAnchor()
    {
        if (Container != null)
        {
            UpdateAnchor(Container.Location);
        }
    }
    /// <summary>
    /// Updates the <see cref="Anchor"/> and applies optimizations if needed based on <see cref="EnableOptimizations"/> flag
    /// </summary>
    /// <param name="location"></param>
    
    protected void UpdateAnchor(Point location)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime applicationLifetime)
        {
            var mainWindowDataContext = applicationLifetime.MainWindow.DataContext;
               
        }
        _lastUpdatedContainerPosition = location;
        if ( Container != null)
        {
            Size containerMargin = (Size)Container.Bounds.Size - (Size)Container.DesiredSize;
            Point relativeLocation = Thumb.TranslatePoint(new Point((Thumb.Bounds.Width - containerMargin.Width)/2,
                (Thumb.Bounds.Height -containerMargin.Height)/2), Container)!.Value;
            Anchor = new Point(location.X + relativeLocation.X, location.Y + relativeLocation.Y);
            ((ConnectorViewModelBase)DataContext).Anchor = Anchor;
        }
    }
  protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Container = this.GetParentOfType<Node>();
        Editor =  this.GetParentOfType<NodifyEditor>();
        Thumb = this.GetChildOfType<Control>("PART_Connector");
        Loaded += OnConnectorLoaded;
        Unloaded += OnConnectorUnloaded;
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = true;

        if (IsConnected)
        {
            OnDisconnect();
        }
        else 
        {
            if (EnableStickyConnections && IsPendingConnection)
            {
                OnConnectorDragCompleted(e.GetPosition(Editor));
            }
            else
            {
                UpdateAnchor();
                OnConnectorDragStarted();
            }
        }
    }

  

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (IsPendingConnection)
        {
            Vector offset = e.GetPosition(Thumb)-_thumbCenter;
            OnConnectorDrag(offset);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = EnableStickyConnections && IsPendingConnection;

        if (!EnableStickyConnections)
        {
            OnConnectorDragCompleted(e.GetPosition(Editor));
            e.Handled = true;
        }
        else if (AllowPendingConnectionCancellation && IsPendingConnection)
        {
            // Cancel pending connection
            OnConnectorDragCompleted(e.GetPosition(Editor),true);
           // ReleaseMouseCapture();

            // Don't show context menu
            e.Handled = true;
        }
    }
    protected virtual void OnDisconnect()
    {
        if (IsConnected && !IsPendingConnection)
        {
            object? connector = DataContext;
            var args = new ConnectorEventArgs(connector)
            {
                RoutedEvent = DisconnectEvent,
                Anchor = Anchor,
                Source = this
            };

            RaiseEvent(args);

            // Raise DisconnectCommand if event is Disconnect not handled
            if (!args.Handled && (DisconnectCommand?.CanExecute(connector) ?? false))
            {
                DisconnectCommand.Execute(connector);
            }
        }
    }
    protected virtual void OnConnectorDrag(Vector offset)
    {
        var args = new PendingConnectionEventArgs(DataContext)
        {
            RoutedEvent = PendingConnectionDragEvent,
            OffsetX = offset.X,
            OffsetY = offset.Y,
            Anchor = Anchor,
            Source = this
        };

        RaiseEvent(args);
    }

    protected virtual void OnConnectorDragStarted()
    {
        if (Thumb != null)
        {
            _thumbCenter = new Point(Thumb.Bounds.Width / 2, Thumb.Bounds.Height / 2);
        }
        var args = new PendingConnectionEventArgs(DataContext)
        {
            RoutedEvent = PendingConnectionStartedEvent,
            Anchor = Anchor,
            Source = this
        };

        RaiseEvent(args);
        IsPendingConnection = !args.Canceled;

        
    }

    protected virtual void OnConnectorDragCompleted(Point point,bool cancel = false)
    {
        if (IsPendingConnection)
        {
            Control? elem = Editor != null ? PendingConnection.GetPotentialConnector(Editor, PendingConnection.GetAllowOnlyConnectorsAttached(Editor),point) : null;

            var args = new PendingConnectionEventArgs(DataContext)
            {
                TargetConnector = elem?.DataContext,
                RoutedEvent = PendingConnectionCompletedEvent,
                Anchor = Anchor,
                Source = this,
                Canceled = cancel
            };

            IsPendingConnection = false;
            RaiseEvent(args);
        }
    }
}