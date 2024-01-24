using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using Nodify.Avalonia.Helpers;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Controls;

public class NodifyEditor : ItemsControl
{
    public static readonly AvaloniaProperty<IDataTemplate> ConnectionTemplateProperty = AvaloniaProperty.Register<NodifyEditor, IDataTemplate>(nameof(ConnectionTemplate));
    
    public static readonly AvaloniaProperty<IDataTemplate> PendingConnectionTemplateProperty = AvaloniaProperty.Register<NodifyEditor, IDataTemplate>(nameof(PendingConnectionTemplate));
    
    public DataTemplate ConnectionTemplate
    {
        get => (DataTemplate)GetValue(ConnectionTemplateProperty);
        set => SetValue(ConnectionTemplateProperty, value);
    }

    public DataTemplate PendingConnectionTemplate
    {
        get => (DataTemplate)GetValue(PendingConnectionTemplateProperty);
        set => SetValue(PendingConnectionTemplateProperty, value);
    }
    private static readonly AvaloniaProperty<double> ZoomProperty = AvaloniaProperty.Register<NodifyEditor, double>(nameof(Zoom),1d);
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
    
     #region Command Dependency Properties

        public static readonly AvaloniaProperty ConnectionCompletedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ConnectionCompletedCommand));
        public static readonly AvaloniaProperty ConnectionStartedCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(ConnectionStartedCommand));
        public static readonly AvaloniaProperty DisconnectConnectorCommandProperty = AvaloniaProperty.Register<NodifyEditor,ICommand>(nameof(DisconnectConnectorCommand));
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
        var renderTransform = new TransformGroup();
        var scaleTransform = new ScaleTransform(Zoom, Zoom);
        ScaleTransform = scaleTransform;
        renderTransform.Children.Add(scaleTransform);
        RenderTransform = renderTransform;
        OffsetXProperty.Changed.Subscribe((x) =>
        {
            ViewTranslateTransform.X = x.NewValue.Value;
        });
        OffsetYProperty.Changed.Subscribe((x) =>
        {
            ViewTranslateTransform.Y = x.NewValue.Value;
        });
    }
   
    protected override void OnApplyTemplate( TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ((Control)Parent).SizeChanged+=(OnSizeChanged);
        var visual = this.GetVisualChildren().First().GetVisualChildren().First().GetVisualChildren().First();
       // ItemsHost =(Panel)this.ItemsPanelRoot;

    }

    

    /// <summary>
    /// 记录上一次鼠标位置
    /// </summary>
    private Point lastMousePosition;
    

    /// <summary>
    /// 标记是否先启动了拖动
    /// </summary>
    private bool isDragging = false;
    
    
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
        // 启动计时
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