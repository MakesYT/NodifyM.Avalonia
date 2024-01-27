using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Nodify.Avalonia.Helpers;

namespace Nodify.Avalonia.Controls;

public class NodifyEditor : SelectingItemsControl
{
    public static readonly AvaloniaProperty<object> PendingConnectionProperty = AvaloniaProperty.Register<NodifyEditor, object>(nameof(PendingConnection));
    
    public object PendingConnection
    {
        get => (object)GetValue(PendingConnectionProperty);
        set => SetValue(PendingConnectionProperty, value);
    }
    
    public static readonly AvaloniaProperty<double> ZoomProperty = AvaloniaProperty.Register<NodifyEditor, double>(nameof(Zoom),1d);
    public static readonly AvaloniaProperty<double> OffsetXProperty = AvaloniaProperty.Register<NodifyEditor, double>(nameof(OffsetX),1d);
    public static readonly AvaloniaProperty<double> OffsetYProperty = AvaloniaProperty.Register<NodifyEditor, double>(nameof(OffsetY),1d);
    public static readonly AvaloniaProperty<TranslateTransform> ViewTranslateTransformProperty = AvaloniaProperty.Register<NodifyEditor, TranslateTransform>(nameof(ViewTranslateTransform),new TranslateTransform());
    public static readonly AvaloniaProperty<IEnumerable> ConnectionsProperty = AvaloniaProperty.Register<NodifyEditor, IEnumerable>(nameof(Connections));
    public TranslateTransform ViewTranslateTransform
    {
        get => (TranslateTransform)GetValue(ViewTranslateTransformProperty);
        set => SetValue(ViewTranslateTransformProperty, value);
    }
    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }
    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }
    public static event ZoomChangedEventHandler? ZoomChanged;
    public  double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }
    public IEnumerable Connections
    {
        get => (IEnumerable)GetValue(ConnectionsProperty);
        set => SetValue(ConnectionsProperty, value);
    }
     #region Cosmetic Dependency Properties

        public static readonly AvaloniaProperty BringIntoViewSpeedProperty = AvaloniaProperty.Register<NodifyEditor,double>(nameof(BringIntoViewSpeed), BoxValue.Double1000);
        public static readonly AvaloniaProperty BringIntoViewMaxDurationProperty = AvaloniaProperty.Register<NodifyEditor,double>(nameof(BringIntoViewMaxDuration), BoxValue.Double1);
        public static readonly AvaloniaProperty DisplayConnectionsOnTopProperty = AvaloniaProperty.Register<NodifyEditor,bool>(nameof(DisplayConnectionsOnTop), BoxValue.False);
        public static readonly AvaloniaProperty DisableAutoPanningProperty = AvaloniaProperty.Register<NodifyEditor,bool>(nameof(DisableAutoPanning), BoxValue.False);
        public static readonly AvaloniaProperty AutoPanSpeedProperty = AvaloniaProperty.Register<NodifyEditor,double>(nameof(AutoPanSpeed), 15d);
        public static readonly AvaloniaProperty AutoPanEdgeDistanceProperty = AvaloniaProperty.Register<NodifyEditor,double>(nameof(AutoPanEdgeDistance), (15d));
        public static readonly StyledProperty<IDataTemplate> ConnectionTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(ConnectionTemplate));
        public static readonly StyledProperty<IDataTemplate> DecoratorTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(DecoratorTemplate));
        public static readonly StyledProperty<IDataTemplate> PendingConnectionTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(PendingConnectionTemplate));
        public static readonly StyledProperty<ControlTheme> SelectionRectangleThemeProperty = AvaloniaProperty.Register<NodifyEditor,ControlTheme>(nameof(SelectionRectangleTheme));
        public static readonly AvaloniaProperty DecoratorContainerStyleProperty = AvaloniaProperty.Register<NodifyEditor,Style>(nameof(DecoratorContainerStyle));

        private static void OnDisableAutoPanningChanged(NodifyEditor d, AvaloniaPropertyChangedEventArgs e)
            => ((NodifyEditor)d).OnDisableAutoPanningChanged((bool)e.NewValue);

        /// <summary>
        /// Gets or sets the maximum animation duration in seconds for bringing a location into view.
        /// </summary>
        public double BringIntoViewMaxDuration
        {
            get => (double)GetValue(BringIntoViewMaxDurationProperty);
            set => SetValue(BringIntoViewMaxDurationProperty, value);
        }

        /// <summary>
        /// Gets or sets the animation speed in pixels per second for bringing a location into view.
        /// </summary>
        /// <remarks>Total animation duration is calculated based on distance and clamped between 0.1 and <see cref="BringIntoViewMaxDuration"/>.</remarks>
        public double BringIntoViewSpeed
        {
            get => (double)GetValue(BringIntoViewSpeedProperty);
            set => SetValue(BringIntoViewSpeedProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to display connections on top of <see cref="ItemContainer"/>s or not.
        /// </summary>
        public bool DisplayConnectionsOnTop
        {
            get => (bool)GetValue(DisplayConnectionsOnTopProperty);
            set => SetValue(DisplayConnectionsOnTopProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to disable the auto panning when selecting or dragging near the edge of the editor configured by <see cref="AutoPanEdgeDistance"/>.
        /// </summary>
        public bool DisableAutoPanning
        {
            get => (bool)GetValue(DisableAutoPanningProperty);
            set => SetValue(DisableAutoPanningProperty, value);
        }

        /// <summary>
        /// Gets or sets the speed used when auto-panning scaled by <see cref="AutoPanningTickRate"/>
        /// </summary>
        public double AutoPanSpeed
        {
            get => (double)GetValue(AutoPanSpeedProperty);
            set => SetValue(AutoPanSpeedProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum distance in pixels from the edge of the editor that will trigger auto-panning.
        /// </summary>
        public double AutoPanEdgeDistance
        {
            get => (double)GetValue(AutoPanEdgeDistanceProperty);
            set => SetValue(AutoPanEdgeDistanceProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> to use when generating a new <see cref="BaseConnection"/>.
        /// </summary>
        public DataTemplate ConnectionTemplate
        {
            get => (DataTemplate)GetValue(ConnectionTemplateProperty);
            set => SetValue(ConnectionTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> to use when generating a new <see cref="DecoratorContainer"/>.
        /// </summary>
        public DataTemplate DecoratorTemplate
        {
            get => (DataTemplate)GetValue(DecoratorTemplateProperty);
            set => SetValue(DecoratorTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> to use for the <see cref="PendingConnection"/>.
        /// </summary>
        public DataTemplate PendingConnectionTemplate
        {
            get => (DataTemplate)GetValue(PendingConnectionTemplateProperty);
            set => SetValue(PendingConnectionTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the style to use for the selection rectangle.
        /// </summary>
        public ControlTheme SelectionRectangleTheme
        {
            get => (ControlTheme)GetValue(SelectionRectangleThemeProperty);
            set => SetValue(SelectionRectangleThemeProperty, value);
        }

        /// <summary>
        /// Gets or sets the style to use for the <see cref="DecoratorContainer"/>.
        /// </summary>
        public Style DecoratorContainerStyle
        {
            get => (Style)GetValue(DecoratorContainerStyleProperty);
            set => SetValue(DecoratorContainerStyleProperty, value);
        }

        #endregion
        #region Auto panning

        private void HandleAutoPanning(object? sender, EventArgs e)
        {
            if (!IsPanning)
            {
                /*Point mousePosition = Mouse.GetPosition(this);
                double edgeDistance = AutoPanEdgeDistance;
                double autoPanSpeed = Math.Min(AutoPanSpeed, AutoPanSpeed * AutoPanningTickRate) / (ViewportZoom * 2);
                double x = ViewportLocation.X;
                double y = ViewportLocation.Y;

                if (mousePosition.X <= edgeDistance)
                {
                    x -= autoPanSpeed;
                }
                else if (mousePosition.X >= ActualWidth - edgeDistance)
                {
                    x += autoPanSpeed;
                }

                if (mousePosition.Y <= edgeDistance)
                {
                    y -= autoPanSpeed;
                }
                else if (mousePosition.Y >= ActualHeight - edgeDistance)
                {
                    y += autoPanSpeed;
                }

                ViewportLocation = new Point(x, y);
                MouseLocation = Mouse.GetPosition(ItemsHost);

                State.HandleAutoPanning(new MouseEventArgs(Mouse.PrimaryDevice, 0));*/
            }
        }

        /// <summary>
        /// Called when the <see cref="DisableAutoPanning"/> changes.
        /// </summary>
        /// <param name="shouldDisable">Whether to enable or disable auto panning.</param>
        protected virtual void OnDisableAutoPanningChanged(bool shouldDisable)
        {
            if (shouldDisable)
            {
                _autoPanningTimer?.Stop();
            }
            else if (_autoPanningTimer == null)
            {
                _autoPanningTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(AutoPanningTickRate),
                    DispatcherPriority.Background, HandleAutoPanning);
            }
            else
            {
                _autoPanningTimer.Interval = TimeSpan.FromMilliseconds(AutoPanningTickRate);
                _autoPanningTimer.Start();
            }
        }

        #endregion
        #region Fields

        /// <summary>
        /// Gets or sets the maximum number of pixels allowed to move the mouse before cancelling the mouse event.
        /// Useful for <see cref="ContextMenu"/>s to appear if mouse only moved a bit or not at all.
        /// </summary>
        public static double HandleRightClickAfterPanningThreshold { get; set; } = 12d;

        /// <summary>
        /// Correct <see cref="ItemContainer"/>'s position after moving if starting position is not snapped to grid.
        /// </summary>
        public static bool EnableSnappingCorrection { get; set; } = true;

        /// <summary>
        /// Gets or sets how often the new <see cref="ViewportLocation"/> is calculated in milliseconds when <see cref="DisableAutoPanning"/> is false.
        /// </summary>
        public static double AutoPanningTickRate { get; set; } = 1;

        /// <summary>
        /// Gets or sets if <see cref="NodifyEditor"/>s should enable optimizations based on <see cref="OptimizeRenderingMinimumContainers"/> and <see cref="OptimizeRenderingZoomOutPercent"/>.
        /// </summary>
        public static bool EnableRenderingContainersOptimizations { get; set; } = true;

        /// <summary>
        /// Gets or sets the minimum number of <see cref="ItemContainer"/>s needed to trigger optimizations when reaching the <see cref="OptimizeRenderingZoomOutPercent"/>.
        /// </summary>
        public static uint OptimizeRenderingMinimumContainers { get; set; } = 700;

        /// <summary>
        /// Gets or sets the minimum zoom out percent needed to start optimizing the rendering for <see cref="ItemContainer"/>s.
        /// Value is between 0 and 1.
        /// </summary>
        public static double OptimizeRenderingZoomOutPercent { get; set; } = 0.3;

        /// <summary>
        /// Gets or sets the margin to add in all directions to the <see cref="ItemsExtent"/> or area parameter when using <see cref="FitToScreen(Rect?)"/>.
        /// </summary>
        public static double FitToScreenExtentMargin { get; set; } = 30;

        /// <summary>
        /// Gets or sets if the current position of containers that are being dragged should not be committed until the end of the dragging operation.
        /// </summary>
        public static bool EnableDraggingContainersOptimizations { get; set; } = true;

        /// <summary>
        /// Tells if the <see cref="NodifyEditor"/> is doing operations on multiple items at once.
        /// </summary>
        public bool IsBulkUpdatingItems { get; protected set; }

        /// <summary>
        /// Gets the panel that holds all the <see cref="ItemContainer"/>s.
        /// </summary>
        protected internal Panel ItemsHost { get; private set; }

       
        private DispatcherTimer? _autoPanningTimer;

        /// <summary>
        /// Gets a list of <see cref="ItemContainer"/>s that are selected.
        /// </summary>
        /// <remarks>Cache the result before using it to avoid extra allocations.</remarks>
        protected internal IReadOnlyList<Node> SelectedContainers
        {
            get
            {
                IList selectedItems = base.SelectedItems;
                var selectedContainers = new List<Node>(selectedItems.Count);

                for (var i = 0; i < selectedItems.Count; i++)
                {
                    selectedContainers.Add((Node)selectedItems[i]);
                }

                return selectedContainers;
            }
        }

        #endregion
     #region Readonly Dependency Properties
     public static readonly DirectProperty<NodifyEditor, Rect> SelectedAreaProperty = AvaloniaProperty.RegisterDirect<NodifyEditor,Rect>(nameof (SelectedArea), o => o.SelectedArea);
     public static readonly DirectProperty<NodifyEditor, bool> IsSelectingProperty = AvaloniaProperty.RegisterDirect<NodifyEditor,bool>(nameof (IsSelecting), o => o.IsSelecting); 
     public static readonly DirectProperty<NodifyEditor, bool> IsPanningProperty = AvaloniaProperty.RegisterDirect<NodifyEditor,bool>(nameof (IsPanning), o => o.IsPanning); 
     public static readonly DirectProperty<NodifyEditor, Point> MouseLocationProperty = AvaloniaProperty.RegisterDirect<NodifyEditor,Point>(nameof (MouseLocation), o => o.MouseLocation); 
       
    

     private void OnIsSelectingChanged(bool value)
     {
         if (value)
             this.OnItemsSelectStarted();
         else
             this.OnItemSelectCompleted();
     }

        private void OnItemSelectCompleted()
        {
            if (ItemsSelectCompletedCommand?.CanExecute(null) ?? false)
                ItemsSelectCompletedCommand.Execute(null);
        }

        private void OnItemsSelectStarted()
        {
            if (ItemsSelectStartedCommand?.CanExecute(null) ?? false)
                ItemsSelectStartedCommand.Execute(null);
        }

        
        private bool _isSelecting;
        private Rect _selectedArea;
        private Point _mouseLocation;
        private bool _isPanning;
        /// <summary>
        /// Gets the currently selected area while <see cref="IsSelecting"/> is true.
        /// </summary>
        public Rect SelectedArea
        {
            get => this._selectedArea;
            internal set
            {
                this.SetAndRaise<Rect>((DirectPropertyBase<Rect>) NodifyEditor.SelectedAreaProperty, ref this._selectedArea, value);
            }
        }

        public bool IsSelecting
        {
            get => this._isSelecting;
            internal set
            {
                if (!this.SetAndRaise<bool>((DirectPropertyBase<bool>) NodifyEditor.IsSelectingProperty, ref this._isSelecting, value))
                    return;
                this.OnIsSelectingChanged(value);
            }
        }

        public bool IsPanning
        {
            get => this._isPanning;
            protected internal set
            {
                this.SetAndRaise<bool>((DirectPropertyBase<bool>) NodifyEditor.IsPanningProperty, ref this._isPanning, value);
            }
        }

        public Point MouseLocation
        {
            get => this._mouseLocation;
            protected set
            {
                this.SetAndRaise<Point>((DirectPropertyBase<Point>) NodifyEditor.MouseLocationProperty, ref this._mouseLocation, value);
            }
        }

        #endregion
     #region Command Dependency Properties

        public static readonly AvaloniaProperty ConnectionCompletedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ConnectionCompletedCommand));
        public static readonly AvaloniaProperty ConnectionStartedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ConnectionStartedCommand));
        public static readonly AvaloniaProperty<ICommand> DisconnectConnectorCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(DisconnectConnectorCommand));
        public static readonly AvaloniaProperty RemoveConnectionCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(RemoveConnectionCommand));
        public static readonly AvaloniaProperty ItemsDragStartedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ItemsDragStartedCommand));
        public static readonly AvaloniaProperty ItemsDragCompletedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ItemsDragCompletedCommand));
        public static readonly AvaloniaProperty ItemsSelectStartedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ItemsSelectStartedCommand));
        public static readonly AvaloniaProperty ItemsSelectCompletedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ItemsSelectCompletedCommand));

        /// <summary>
        /// Invoked when the <see cref="PendingConnection"/> is completed. <br />
        /// Use <see cref="PendingConnection.StartedCommand"/> if you want to control the visibility of the connection from the viewmodel. <br />
        /// Parameter is <see cref="PendingConnection.Source"/>.
        /// </summary>
        public ICommand? ConnectionStartedCommand
        {
            get => (ICommand?)GetValue(ConnectionStartedCommandProperty);
            set => SetValue(ConnectionStartedCommandProperty, value);
        }

        /// <summary>
        /// Invoked when the <see cref="PendingConnection"/> is completed. <br />
        /// Use <see cref="PendingConnection.CompletedCommand"/> if you want to control the visibility of the connection from the viewmodel. <br />
        /// Parameter is <see cref="Tuple{T, U}"/> where <see cref="Tuple{T, U}.Item1"/> is the <see cref="PendingConnection.Source"/> and <see cref="Tuple{T, U}.Item2"/> is <see cref="PendingConnection.Target"/>.
        /// </summary>
        public ICommand? ConnectionCompletedCommand
        {
            get => (ICommand?)GetValue(ConnectionCompletedCommandProperty);
            set => SetValue(ConnectionCompletedCommandProperty, value);
        }

        /// <summary>
        /// Invoked when the <see cref="Connector.Disconnect"/> event is raised. <br />
        /// Can also be handled at the <see cref="Connector"/> level using the <see cref="Connector.DisconnectCommand"/> command. <br />
        /// Parameter is the <see cref="Connector"/>'s <see cref="FrameworkElement.DataContext"/>.
        /// </summary>
        public ICommand? DisconnectConnectorCommand
        {
            get => (ICommand?)GetValue(DisconnectConnectorCommandProperty);
            set => SetValue(DisconnectConnectorCommandProperty, value);
        }

        /// <summary>
        /// Invoked when the <see cref="BaseConnection.Disconnect"/> event is raised. <br />
        /// Can also be handled at the <see cref="BaseConnection"/> level using the <see cref="BaseConnection.DisconnectCommand"/> command. <br />
        /// Parameter is the <see cref="BaseConnection"/>'s <see cref="FrameworkElement.DataContext"/>.
        /// </summary>
        public ICommand? RemoveConnectionCommand
        {
            get => (ICommand?)GetValue(RemoveConnectionCommandProperty);
            set => SetValue(RemoveConnectionCommandProperty, value);
        }

        /// <summary>
        /// Invoked when a drag operation starts for the <see cref="SelectedItems"/>.
        /// </summary>
        public ICommand? ItemsDragStartedCommand
        {
            get => (ICommand?)GetValue(ItemsDragStartedCommandProperty);
            set => SetValue(ItemsDragStartedCommandProperty, value);
        }

        /// <summary>
        /// Invoked when a drag operation is completed for the <see cref="SelectedItems"/>.
        /// </summary>
        public ICommand? ItemsDragCompletedCommand
        {
            get => (ICommand?)GetValue(ItemsDragCompletedCommandProperty);
            set => SetValue(ItemsDragCompletedCommandProperty, value);
        }

        /// <summary>Invoked when a selection operation is started.</summary>
        public ICommand? ItemsSelectStartedCommand
        {
            get => (ICommand?)GetValue(ItemsSelectStartedCommandProperty);
            set => SetValue(ItemsSelectStartedCommandProperty, value);
        }

        /// <summary>Invoked when a selection operation is completed.</summary>
        public ICommand? ItemsSelectCompletedCommand
        {
            get => (ICommand?)GetValue(ItemsSelectCompletedCommandProperty);
            set => SetValue(ItemsSelectCompletedCommandProperty, value);
        }

        #endregion
    public NodifyEditor()
    {

        AddHandler(Connector.DisconnectEvent, new ConnectorEventHandler(OnConnectorDisconnected));
        AddHandler(Connector.PendingConnectionStartedEvent, new PendingConnectionEventHandler(OnConnectionStarted));
        AddHandler(Connector.PendingConnectionCompletedEvent, new PendingConnectionEventHandler(OnConnectionCompleted));
        
        AddHandler(BaseConnection.DisconnectEvent, new ConnectionEventHandler(OnRemoveConnection));
        PointerReleased += OnPointerReleased;
        PointerWheelChanged+=OnPointerWheelChanged;
        PointerPressed += OnPointerPressed;
        PointerMoved+=OnPointerMoved;
        DisableAutoPanningProperty.Changed.AddClassHandler<NodifyEditor>(OnDisableAutoPanningChanged);
        var renderTransform = new TransformGroup();
        var scaleTransform = new ScaleTransform(Zoom, Zoom);
        ScaleTransform = scaleTransform;
        renderTransform.Children.Add(scaleTransform);
        RenderTransform = renderTransform;
        OffsetXProperty.Changed.Subscribe(x =>
        {
            ViewTranslateTransform.X = x.NewValue.Value;
        });
        OffsetYProperty.Changed.Subscribe(x =>
        {
            ViewTranslateTransform.Y = x.NewValue.Value;
        });
    }
   
    protected override void OnApplyTemplate( TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ((Control)Parent).SizeChanged+=(OnSizeChanged);
        var visual = this.GetVisualChildren().First().GetVisualChildren().First().GetVisualChildren().First();
        SelectedArea = new Rect();
        // ItemsHost =(Panel)this.ItemsPanelRoot;

    }

    

    /// <summary>
    /// 记录上一次鼠标位置
    /// </summary>
    private Point lastMousePosition;
    

    /// <summary>
    /// 标记是否先启动了拖动
    /// </summary>
    private bool isDragging;
    
    
    private double _startOffsetX;
    private double _startOffsetY;

    private ScaleTransform ScaleTransform;
    
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        foreach (var visualChild in this.GetVisualChildren())
        {
            if (visualChild.GetVisualChildren().First() is Node n)
            {
                n.IsSelected = false;
            }
        }
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        // 启动拖动
        isDragging = true;
        // 记录当前坐标
        lastMousePosition = e.GetPosition(this);
        // Debug.WriteLine($"记录当前坐标X:{lastMousePosition.X} Y:{lastMousePosition.Y}");
        _startOffsetX = OffsetX;
        _startOffsetY = OffsetY;
        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!isDragging) return;
        // 停止拖动
        isDragging = false;
        e.Handled = true;
        // 停止计时器
        
        // var currentPoint = e.GetCurrentPoint(this);
        //  Debug.WriteLine($"停止拖动坐标X:{OffsetX} Y:{OffsetY}");
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        
        // 如果没有启动拖动，则不执行
        if (!isDragging) return;

        var currentMousePosition = e.GetPosition(this);
        var offset = currentMousePosition - lastMousePosition;
        
        //lastMousePosition = e.GetPosition(this);
        // 记录当前坐标
        OffsetX = offset.X+_startOffsetX;
        
        OffsetY = offset.Y+_startOffsetY;
        
    }
    

    private double _initWeight;
    private double _initHeight;
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _initWeight = e.NewSize.Width;
        _initHeight = e.NewSize.Height;
        Width = _initWeight / Zoom;
        Height = _initHeight / Zoom;
        e.Handled = true;
    }
    private bool isZooming = false;

    private double _nowScale = 1;
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs pointerWheelEventArgs)
    {
        var position = pointerWheelEventArgs.GetPosition(this);
        var deltaY = pointerWheelEventArgs.Delta.Y;
        if (deltaY < 0)
        {
            _nowScale *= 0.9d;
            _nowScale = Math.Max(0.1d, _nowScale);
        }
        else
        {
            _nowScale *= 1.1d;
            _nowScale = Math.Min(10d, _nowScale);
        }
        OffsetX += (Zoom - _nowScale) * position.X / _nowScale; 
        OffsetY += (Zoom - _nowScale) * position.Y / _nowScale;
        Zoom = _nowScale;
        Width = _initWeight / Zoom;
        Height = _initHeight /  Zoom;
        ScaleTransform.ScaleX = Zoom;
        ScaleTransform.ScaleY = Zoom;
        
        pointerWheelEventArgs.Handled = true;
    }
    #region Connector handling

    private void OnConnectorDisconnected(object sender, ConnectorEventArgs e)
    {
        if (!e.Handled && (DisconnectConnectorCommand?.CanExecute(e.Connector) ?? false))
        {
            DisconnectConnectorCommand.Execute(e.Connector);
            e.Handled = true;
        }
    }

    private void OnConnectionStarted(object sender, PendingConnectionEventArgs e)
    {
        if (!e.Canceled && ConnectionStartedCommand != null)
        {
            e.Canceled = !ConnectionStartedCommand.CanExecute(e.SourceConnector);
            if (!e.Canceled)
            {
                ConnectionStartedCommand.Execute(e.SourceConnector);
            }
        }
    }

    private void OnConnectionCompleted(object sender, PendingConnectionEventArgs e)
    {
        if (!e.Canceled)
        {
            (object SourceConnector, object? TargetConnector) result = (e.SourceConnector, e.TargetConnector);
            if (ConnectionCompletedCommand?.CanExecute(result) ?? false)
            {
                ConnectionCompletedCommand.Execute(result);
            }
        }
    }

    private void OnRemoveConnection(object sender, ConnectionEventArgs e)
    {
        if (RemoveConnectionCommand?.CanExecute(e.Connection) ?? false)
        {
            RemoveConnectionCommand.Execute(e.Connection);
        }
    }

    #endregion
}