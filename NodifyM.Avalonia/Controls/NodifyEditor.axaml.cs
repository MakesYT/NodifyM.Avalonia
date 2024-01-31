using System.Collections;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;

namespace NodifyM.Avalonia.Controls;

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
    public static readonly StyledProperty<TranslateTransform> ViewTranslateTransformProperty = AvaloniaProperty.Register<NodifyEditor, TranslateTransform>(nameof(ViewTranslateTransform),new TranslateTransform());
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
    
    public static readonly StyledProperty<IDataTemplate> ConnectionTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(ConnectionTemplate));
    public static readonly StyledProperty<IDataTemplate> DecoratorTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(DecoratorTemplate));
    public static readonly StyledProperty<IDataTemplate> PendingConnectionTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(PendingConnectionTemplate));
    public static readonly StyledProperty<IDataTemplate> GridLineTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(GridLineTemplate));
    
    public static readonly AvaloniaProperty DecoratorContainerStyleProperty = AvaloniaProperty.Register<NodifyEditor,Style>(nameof(DecoratorContainerStyle));

       
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
    public DataTemplate GridLineTemplate
    {
        get => (DataTemplate)GetValue(GridLineTemplateProperty);
        set => SetValue(GridLineTemplateProperty, value);
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
        
    
    #region Command Dependency Properties

    public static readonly AvaloniaProperty ConnectionCompletedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ConnectionCompletedCommand));
    public static readonly AvaloniaProperty ConnectionStartedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ConnectionStartedCommand));
    public static readonly AvaloniaProperty<ICommand> DisconnectConnectorCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(DisconnectConnectorCommand));
    public static readonly AvaloniaProperty RemoveConnectionCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(RemoveConnectionCommand));
        

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
        

    #endregion
    public NodifyEditor()
    {

        AddHandler(Connector.DisconnectEvent, new ConnectorEventHandler(OnConnectorDisconnected));
        AddHandler(Connector.PendingConnectionStartedEvent, new PendingConnectionEventHandler(OnConnectionStarted));
        AddHandler(Connector.PendingConnectionCompletedEvent, new PendingConnectionEventHandler(OnConnectionCompleted));
        
        AddHandler(BaseConnection.DisconnectEvent, new ConnectionEventHandler(OnRemoveConnection));
        
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ((Control)Parent).SizeChanged += (OnSizeChanged);
        PointerReleased += OnPointerReleased;
        PointerWheelChanged += OnPointerWheelChanged;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        var renderTransform = new TransformGroup();
        var scaleTransform = new ScaleTransform(Zoom, Zoom);
        ScaleTransform = scaleTransform;
        renderTransform.Children.Add(scaleTransform);
        RenderTransform = renderTransform;
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
        var visual = ((Control)sender).GetLogicalChildren();
        foreach (var visualChild in visual)
        {
            
            if (visualChild.GetLogicalChildren().First() is Node n)
            {
                n.IsSelected = false;
            }
        }
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        isDragging = true;
        lastMousePosition = e.GetPosition(this);
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
        ViewTranslateTransform.X = OffsetX;
        OffsetY = offset.Y+_startOffsetY;
        ViewTranslateTransform.Y = OffsetY;
        
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
        ViewTranslateTransform.X = OffsetX;
        OffsetY += (Zoom - _nowScale) * position.Y / _nowScale;
        ViewTranslateTransform.Y = OffsetY;
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


    #region AlignNode

    public static readonly AvaloniaProperty<int> AlignmentRangeProperty = AvaloniaProperty.Register<NodifyEditor, int>(nameof(AlignmentRange),10);
    public static readonly AvaloniaProperty<bool> AllowAlignProperty = AvaloniaProperty.Register<NodifyEditor, bool>(nameof(AllowAlign),BoxValue.True);
    
    public static readonly StyledProperty<IDataTemplate> AlignmentLineTemplateProperty = AvaloniaProperty.Register<NodifyEditor,IDataTemplate>(nameof(AlignmentLineTemplate));
    public static readonly StyledProperty<AvaloniaList<object>> AlignmentLineProperty = AvaloniaProperty.Register<NodifyEditor, AvaloniaList<object>>(nameof(AlignmentLine));
    public AvaloniaList<object> AlignmentLine
    {
        get => GetValue(AlignmentLineProperty);
        set => SetValue(AlignmentLineProperty, value);
    }
    public IDataTemplate AlignmentLineTemplate
    {
        get => (DataTemplate)GetValue(AlignmentLineTemplateProperty);
        set => SetValue(AlignmentLineTemplateProperty, value);
    }
    
    public int AlignmentRange
    {
        get => (int)GetValue(AlignmentRangeProperty);
        set => SetValue(AlignmentRangeProperty, value);
    }
    public bool AllowAlign
    {
        get => (bool)GetValue(AllowAlignProperty);
        set => SetValue(AllowAlignProperty, value);
    }
    public Point TryAlignNode(BaseNode control,Point point)
    {
        AlignmentLine.Clear();
        
        if (!AllowAlign) return point;
        double x = (int)point.X;
        double y = (int)point.Y;
        double nowIntervalX = AlignmentRange;
        double nowIntervalY = AlignmentRange;

        if (ItemsPanelRoot?.Children == null) return point;
        foreach (var child in ItemsPanelRoot?.Children)
        {
            var node = (BaseNode)child.GetVisualChildren().First();
            if (node == control)
            {
                continue;
            }

            // 合并两个区域的代码
            var regionX = node.Location.X;
            var regionY = node.Location.Y;
            var controlWidth = control.Bounds.Width;
            var controlHeight = control.Bounds.Height;

            // 计算左上角区域的边界
            var intervalX = Math.Abs(regionX - x);
            if (intervalX < nowIntervalX)
            {
                x = regionX;
                
                nowIntervalX = intervalX;
            }

            var intervalX2 = Math.Abs(regionX + node.Bounds.Width - x);
            if (intervalX2 < nowIntervalX)
            {
                x = regionX + node.Bounds.Width;
                nowIntervalX = intervalX2;
            }

            var intervalY = Math.Abs(regionY - y);
            if (intervalY < nowIntervalY)
            {
                y = regionY;
                nowIntervalY = intervalY;
            }

            var intervalY2 = Math.Abs(regionY + node.Bounds.Height - y);
            if (intervalY2 < nowIntervalY)
            {
                y = regionY + node.Bounds.Height;
                nowIntervalY = intervalY2;
            }

            // 计算右下角区域的边界
            var intervalX3 = Math.Abs(regionX - controlWidth - x);
            if (intervalX3 < nowIntervalX)
            {
                x = regionX - controlWidth;
                nowIntervalX = intervalX3;
            }

            var intervalX4 = Math.Abs(regionX - controlWidth + node.Bounds.Width - x);
            if (intervalX4 < nowIntervalX)
            {
                x = regionX - controlWidth + node.Bounds.Width;
                nowIntervalX = intervalX4;
            }

            var intervalY3 = Math.Abs(regionY - controlHeight - y);
            if (intervalY3 < nowIntervalY)
            {
                y = regionY - controlHeight;
                nowIntervalY = intervalY3;
            }

            var intervalY4 = Math.Abs(regionY - controlHeight + node.Bounds.Height - y);
            if (intervalY4 < nowIntervalY)
            {
                y = regionY - controlHeight + node.Bounds.Height;
                nowIntervalY = intervalY4;
            }
        }
        
        return new Point(x, y);

    }

    #endregion
}