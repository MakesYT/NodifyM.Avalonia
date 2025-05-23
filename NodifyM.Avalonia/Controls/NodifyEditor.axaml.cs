﻿using System.Collections;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;
using NodifyM.Avalonia.ViewModelBase;

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
    public static readonly StyledProperty<TranslateTransform> ViewTranslateTransformProperty = AvaloniaProperty.Register<NodifyEditor, TranslateTransform>(nameof(ViewTranslateTransform));
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
        AddHandler(BaseNode.LocationChangedEvent,new NodeLocationEventHandler( OnNodeLocationChanged));
        AddHandler(BaseConnection.DisconnectEvent, new ConnectionEventHandler(OnRemoveConnection));
    }
    public void SelectItem(BaseNode node)
    {
        var visual = this.GetChildOfType<Canvas>("NodeItemsPresenter").Children;
        foreach (var visualChild in visual)
        {
            visualChild.ZIndex = 0;
            if (visualChild.GetChildOfType<BaseNode>() is BaseNode n)
            {
                n.IsSelected = false;
            }
            
        }
        node.IsSelected = true;
        node.GetVisualParent().ZIndex = 1;
        SelectedItem = node.DataContext;
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
        AutoPanningTimer=new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal,HandleAutoPanning);
        AutoPanningTimer.Stop();
        
        AlignmentLine = new AvaloniaList<object>();
        
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var T = 0.0d;
        var L = 0.0d;
        var B = 0d;
        var R = 0d;
        var childOfType = this.GetChildOfType<Canvas>("NodeItemsPresenter");
        foreach (var logicalChild in childOfType.GetVisualChildren())
        {
            var logicalChildLogicalChild = ((BaseNode)logicalChild.GetVisualChildren().First());
            var location = logicalChildLogicalChild.Location;
            if (location.Y< T)
            {
                T = location.Y;
            }
            if (location.X < L)
            {
                L = location.X;
            }
            if (location.Y+ logicalChildLogicalChild.Bounds.Height> B)
            {
                B = location.Y+ logicalChildLogicalChild.Bounds.Height;
            }
            if (location.X + logicalChildLogicalChild.Bounds.Width > R)
            {
                R = location.X + logicalChildLogicalChild.Bounds.Width;
            }
            
        }
        ViewTranslateTransform = new TranslateTransform(-L,-T);
        OffsetY = -T;
        OffsetX = -L;
        if (1/(Math.Abs(T - B) / _initHeight)<ScaleTransform.ScaleY)
        {
            ScaleTransform.ScaleY = 1/(Math.Abs(T - B) / _initHeight);
            ScaleTransform.ScaleX = 1/(Math.Abs(T - B) / _initHeight);
        }
        if (1/(Math.Abs(L - R) / _initWeight)<ScaleTransform.ScaleY)
        {
            ScaleTransform.ScaleY =  1/(Math.Abs(L - R) / _initWeight);
            ScaleTransform.ScaleX =  1/(Math.Abs(L - R) / _initWeight);
        }
        Zoom = ScaleTransform.ScaleY;
        _nowScale = Zoom;
        Width = _initWeight / Zoom;
        Height = _initHeight /  Zoom;
        
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
        if (e.Handled)
        {
            return;
        }
        var visual = ((Control)sender).GetLogicalChildren();
        foreach (var visualChild in visual)
        {
            
            if (visualChild.GetLogicalChildren().First() is Node n)
            {
                n.IsSelected = false;
            }
        }
        SelectedItem=null;
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
        AutoPanningTimer.Stop();
        // var currentPoint = e.GetCurrentPoint(this);
        //  Debug.WriteLine($"停止拖动坐标X:{OffsetX} Y:{OffsetY}");
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }
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
    public void ClearAlignmentLine()
    {
        AlignmentLine.Clear();
    }
    public Point TryAlignNode(BaseNode control,Point point)
    {
        AlignmentLine.Clear();
        
        if (!AllowAlign) return point;
        double x = (int)point.X;
        double y = (int)point.Y;
        double nowIntervalX = AlignmentRange;
        double nowIntervalY = AlignmentRange;
        var movingNodeWidth = control.Bounds.Width;
        var movingNodeHeight = control.Bounds.Height;
        if (ItemsPanelRoot?.Children == null) return point;
        foreach (var child in ItemsPanelRoot?.Children)
        {
            var node = (BaseNode)child.GetVisualChildren().First();
            if (node == control)
            {
                continue;
            }
            
            var nodeLocationX = node.Location.X;
            var nodeLocationY = node.Location.Y;
            var nodeWidth = node.Bounds.Width;
            var nodeHeight = node.Bounds.Height;
            
            
            //上->上
            var intervalY = Math.Abs(nodeLocationY - y);
            if (intervalY <= nowIntervalY)
            {
                y = nodeLocationY;
                nowIntervalY = intervalY;
                AlignmentLine.Add(x <= nodeLocationX
                    ? new AlignmentLineViewModel(new Point(nodeLocationX + nodeWidth, y ), new Point(control.Location.X, y ))
                    : new AlignmentLineViewModel(new Point(nodeLocationX, y ),
                        new Point(control.Location.X + movingNodeWidth, y )));
            }
            //上->下
            var intervalY3 = Math.Abs(nodeLocationY - movingNodeHeight - y);
            if (intervalY3 <= nowIntervalY)
            {
                y = nodeLocationY - movingNodeHeight;
                nowIntervalY = intervalY3;
                AlignmentLine.Add(x <= nodeLocationX
                    ? new AlignmentLineViewModel(new Point(nodeLocationX + nodeWidth, nodeLocationY ), new Point(control.Location.X, nodeLocationY ))
                    : new AlignmentLineViewModel(new Point(nodeLocationX, nodeLocationY ),
                        new Point(control.Location.X + movingNodeWidth, nodeLocationY )));
            }
            //下->下
            var intervalY4 = Math.Abs(nodeLocationY - movingNodeHeight + nodeHeight - y);
            if (intervalY4 <= nowIntervalY)
            {
                y = nodeLocationY - movingNodeHeight + nodeHeight;
                nowIntervalY = intervalY4;
                AlignmentLine.Add(x <= nodeLocationX
                    ? new AlignmentLineViewModel(new Point(nodeLocationX + nodeWidth, y+movingNodeHeight ), new Point(control.Location.X, y+movingNodeHeight ))
                    : new AlignmentLineViewModel(new Point(nodeLocationX, y+movingNodeHeight ),
                        new Point(control.Location.X + movingNodeWidth, y+movingNodeHeight )));
            }
            //下->上
            var intervalY2 = Math.Abs(nodeLocationY + nodeHeight - y);
            if (intervalY2 <= nowIntervalY)
            {
               
                y = nodeLocationY + nodeHeight;
                nowIntervalY = intervalY2;
                AlignmentLine.Add(x <= nodeLocationX
                    ? new AlignmentLineViewModel(new Point(nodeLocationX + nodeWidth, y ), new Point(control.Location.X, y ))
                    : new AlignmentLineViewModel(new Point(nodeLocationX, y ),
                        new Point(control.Location.X + movingNodeWidth, y )));
            }
            //左->右
            var intervalX3 = Math.Abs(nodeLocationX - movingNodeWidth - x);
            if (intervalX3 <= nowIntervalX)
            {
                x = nodeLocationX - movingNodeWidth;
                nowIntervalX = intervalX3;
                AlignmentLine.Add(y <= nodeLocationY
                    ? new AlignmentLineViewModel(new Point(x+movingNodeWidth, control.Location.Y), 
                        new Point(x+movingNodeWidth, nodeLocationY+nodeHeight))
                    : new AlignmentLineViewModel(new Point(x+movingNodeWidth, control.Location.Y+movingNodeHeight),
                        new Point(x+movingNodeWidth, nodeLocationY)));
            }
            
            //左->左
            var intervalX = Math.Abs(nodeLocationX - x);
            if (intervalX <= nowIntervalX)
            {
                x = nodeLocationX;
                nowIntervalX = intervalX;
                AlignmentLine.Add(y <= nodeLocationY
                    ? new AlignmentLineViewModel(new Point(x, control.Location.Y), 
                        new Point(x, nodeLocationY+nodeHeight))
                    : new AlignmentLineViewModel(new Point(x, control.Location.Y+movingNodeHeight),
                        new Point(x, nodeLocationY)));
            }
            //右->右
            var intervalX4 = Math.Abs(nodeLocationX - movingNodeWidth + nodeWidth - x);
            if (intervalX4 <= nowIntervalX)
            {
                x = nodeLocationX - movingNodeWidth + nodeWidth;
                nowIntervalX = intervalX4;
                AlignmentLine.Add(y <= nodeLocationY
                    ? new AlignmentLineViewModel(new Point(x+movingNodeWidth, control.Location.Y), 
                        new Point(x+movingNodeWidth, nodeLocationY+nodeHeight))
                    : new AlignmentLineViewModel(new Point(x+movingNodeWidth, control.Location.Y+movingNodeHeight),
                        new Point(x+movingNodeWidth, nodeLocationY)));
            }

            //右->左
            var intervalX2 = Math.Abs(nodeLocationX + nodeWidth - x);
            if (intervalX2 <= nowIntervalX)
            {
                x = nodeLocationX + nodeWidth;
                nowIntervalX = intervalX2;
                AlignmentLine.Add(y <= nodeLocationY
                    ? new AlignmentLineViewModel(new Point(x, control.Location.Y), 
                        new Point(x, nodeLocationY+nodeHeight))
                    : new AlignmentLineViewModel(new Point(x, control.Location.Y+movingNodeHeight),
                        new Point(x, nodeLocationY)));
            }
            //竖中
            var intervalX5 = Math.Abs((nodeLocationX + nodeWidth/2) - (x + movingNodeWidth/2));
            if (intervalX5 <= nowIntervalX)
            {
                x = (nodeLocationX + nodeWidth/2)-movingNodeWidth/2;
                nowIntervalX = intervalX5;
                AlignmentLine.Add(y <= nodeLocationY
                    ? new AlignmentLineViewModel(new Point(x+movingNodeWidth/2, control.Location.Y), 
                        new Point(x+movingNodeWidth/2, nodeLocationY+nodeHeight))
                    : new AlignmentLineViewModel(new Point(x+movingNodeWidth/2, control.Location.Y+movingNodeHeight),
                        new Point(x+movingNodeWidth/2, nodeLocationY)));
            }
            // 横中
            var intervalY5 = Math.Abs(nodeLocationY + nodeHeight/2 - y-movingNodeHeight/2);
            if (intervalY5 <= nowIntervalY)
            {
               
                y = nodeLocationY + nodeHeight/2-movingNodeHeight/2;
                nowIntervalY = intervalY5;
                AlignmentLine.Add(x <= nodeLocationX
                    ? new AlignmentLineViewModel(new Point(nodeLocationX + nodeWidth, y+movingNodeHeight/2 ), new Point(control.Location.X, y +movingNodeHeight/2))
                    : new AlignmentLineViewModel(new Point(nodeLocationX, y+movingNodeHeight/2 ),
                        new Point(control.Location.X + movingNodeWidth, y+movingNodeHeight/2 )));
            }

        }

        for (var index = AlignmentLine.Count - 1; index >= 0; index--)
        {
            var o = AlignmentLine[index];
            if (o is AlignmentLineViewModel alignmentLineViewModel)
            {
                if (alignmentLineViewModel.Start.X.Equals(alignmentLineViewModel.End.X))
                {
                    //竖向
                    if (!alignmentLineViewModel.Start.X.Equals(x)&&!alignmentLineViewModel.Start.X.Equals(x+movingNodeWidth)&&!alignmentLineViewModel.Start.X.Equals(x+movingNodeWidth/2))
                    {
                        AlignmentLine.RemoveAt(index);
                    }
                }

                if (alignmentLineViewModel.Start.Y.Equals(alignmentLineViewModel.End.Y))
                {
                    //横向
                    if (!alignmentLineViewModel.Start.Y.Equals(y)&&!alignmentLineViewModel.Start.Y.Equals(y+movingNodeHeight)&&!alignmentLineViewModel.Start.Y.Equals(y+movingNodeHeight/2))
                    {
                        AlignmentLine.RemoveAt(index);
                    }
                }
            }
        }

        return new Point(x, y);

    }
    

    #endregion

    #region Auto Panning
    
    public static readonly AvaloniaProperty<bool> AllowAutoPanningProperty = AvaloniaProperty.Register<NodifyEditor, bool>(nameof(AllowAutoPanning),BoxValue.True);
    public static readonly AvaloniaProperty<int> AutoPanningSpeedProperty = AvaloniaProperty.Register<NodifyEditor, int>(nameof(AutoPanningSpeed),10);
    public static readonly AvaloniaProperty<double> AutoPanningXEdgeDistanceProperty = AvaloniaProperty.Register<NodifyEditor, double>(nameof(AutoPanningXEdgeDistance),0.02); 
    public static readonly AvaloniaProperty<double> AutoPanningYEdgeDistanceProperty = AvaloniaProperty.Register<NodifyEditor, double>(nameof(AutoPanningYEdgeDistance),0.04);
    public double AutoPanningXEdgeDistance
    {
        get => (double)GetValue(AutoPanningXEdgeDistanceProperty);
        set => SetValue(AutoPanningXEdgeDistanceProperty, value);
    }
    public double AutoPanningYEdgeDistance
    {
        get => (double)GetValue(AutoPanningYEdgeDistanceProperty);
        set => SetValue(AutoPanningYEdgeDistanceProperty, value);
    }
    
    public int AutoPanningSpeed
    {
        get => (int)GetValue(AutoPanningSpeedProperty);
        set => SetValue(AutoPanningSpeedProperty, value);
    }
    public bool AllowAutoPanning
    {
        get => (bool)GetValue(AllowAutoPanningProperty);
        set => SetValue(AllowAutoPanningProperty, value);
    }
    
    
    public static readonly RoutedEvent NodifyAutoPanningEvent = RoutedEvent.Register<NodifyAutoPanningEventArgs>(nameof(NodifyAutoPanning), RoutingStrategies.Bubble, typeof(NodifyEditor));
    public event NodifyAutoPanningEventHandler NodifyAutoPanning
    {
        add => AddHandler(NodifyAutoPanningEvent, value);
        remove => RemoveHandler(NodifyAutoPanningEvent, value);
    }
    DispatcherTimer AutoPanningTimer;
    NodeLocationEventArgs locationChangedEventArgs;
    BaseNode baseNode;
    private void OnNodeLocationChanged(object? sender, NodeLocationEventArgs e)
    {
        if (!AllowAutoPanning)
        {
            return;
        }
        baseNode= e.Sender;
        locationChangedEventArgs = e;
        if (!AutoPanningTimer.IsEnabled)
        {
            AutoPanningTimer.Start();
        }

        if (e.Stop)
        {
            AutoPanningTimer.Stop();
        }
       
    }

    private void HandleAutoPanning(object? sender, EventArgs eventArgs)
    {
        var offset = 10/Zoom;
        if (OffsetX+locationChangedEventArgs.Location.X<Bounds.Width*AutoPanningXEdgeDistance)
        {
            OffsetX += offset;
            ViewTranslateTransform.X = OffsetX;
            baseNode.Location += new Point(-offset,0);
            locationChangedEventArgs.Location+=new Point(-offset,0);
            RaiseEvent(new NodifyAutoPanningEventArgs(NodifyAutoPanningEvent, baseNode));
        }else if (OffsetX+baseNode.Bounds.Width+locationChangedEventArgs.Location.X>Bounds.Width*(1-AutoPanningXEdgeDistance))
        {
            OffsetX -= offset;
            ViewTranslateTransform.X = OffsetX;
            baseNode.Location += new Point(offset,0);
            locationChangedEventArgs.Location+=new Point(offset,0);
            RaiseEvent(new NodifyAutoPanningEventArgs(NodifyAutoPanningEvent, baseNode));
        }else if (OffsetY+locationChangedEventArgs.Location.Y<Bounds.Height*AutoPanningYEdgeDistance)
        {
            OffsetY += offset;
            ViewTranslateTransform.Y = OffsetY;
            baseNode.Location += new Point(0,-offset);
            locationChangedEventArgs.Location+=new Point(0,-offset);
            RaiseEvent(new NodifyAutoPanningEventArgs(NodifyAutoPanningEvent, baseNode));
        }else if (OffsetY+baseNode.Bounds.Height+locationChangedEventArgs.Location.Y>Bounds.Height* (1-AutoPanningYEdgeDistance))
        {
            OffsetY -= offset;
            ViewTranslateTransform.Y = OffsetY;
            baseNode.Location += new Point(0,offset);
            locationChangedEventArgs.Location += new Point(0, offset);
            RaiseEvent(new NodifyAutoPanningEventArgs(NodifyAutoPanningEvent, baseNode));
        }
        else
        {
            AutoPanningTimer.Stop();
        }
    }
    #endregion
}