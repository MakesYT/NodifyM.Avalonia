using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Nodify.Avalonia.Helpers;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Controls;

public class Node : ContentControl
{
    public static readonly AvaloniaProperty<IBrush> ContentBrushProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(ContentBrush));
    public static readonly AvaloniaProperty<IBrush> HeaderBrushProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(HeaderBrush));
    public static readonly AvaloniaProperty<IBrush> FooterBrushProperty = AvaloniaProperty.Register<Node, IBrush>(nameof(FooterBrush));
    public static readonly AvaloniaProperty<object> FooterProperty = AvaloniaProperty.Register<Node, object>(nameof(Footer));
    public static readonly AvaloniaProperty<object> HeaderProperty = AvaloniaProperty.Register<Node, object>(nameof(Header));
    public static readonly AvaloniaProperty<IDataTemplate> FooterTemplateProperty = AvaloniaProperty.Register<Node, IDataTemplate>(nameof(FooterTemplate));
    public static readonly AvaloniaProperty<IDataTemplate> HeaderTemplateProperty = AvaloniaProperty.Register<Node, IDataTemplate>(nameof(HeaderTemplate));
    public static readonly AvaloniaProperty<IDataTemplate> InputConnectorTemplateProperty = AvaloniaProperty.Register<Node, IDataTemplate>(nameof(InputConnectorTemplate));
    protected internal static readonly AvaloniaProperty<bool> HasFooterProperty = AvaloniaProperty.RegisterDirect<Node, bool>(nameof(HasFooter), o => o.HasFooter);
    protected internal static readonly AvaloniaProperty<bool> HasHeaderProperty = AvaloniaProperty.RegisterDirect<Node, bool>(nameof(HasHeader), o => o.HasHeader);
    public static readonly AvaloniaProperty<IDataTemplate> OutputConnectorTemplateProperty = AvaloniaProperty.Register<Node, IDataTemplate>(nameof(OutputConnectorTemplate));
    public static readonly AvaloniaProperty<IEnumerable> InputProperty = AvaloniaProperty.Register<Node, IEnumerable>(nameof(Input));
    public static readonly AvaloniaProperty<IEnumerable> OutputProperty = AvaloniaProperty.Register<Node, IEnumerable>(nameof(Output));
    public static readonly AvaloniaProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Node, bool>(nameof(IsSelected));
    
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    public Brush ContentBrush
    {
        get => (Brush)GetValue(ContentBrushProperty);
        set => SetValue(ContentBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for the background of the <see cref="HeaderedContentControl.Header"/> of this <see cref="Node"/>.
    /// </summary>
    public Brush HeaderBrush
    {
        get => (Brush)GetValue(HeaderBrushProperty);
        set => SetValue(HeaderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for the background of the <see cref="Node.Footer"/> of this <see cref="Node"/>.
    /// </summary>
    public Brush FooterBrush
    {
        get => (Brush)GetValue(FooterBrushProperty);
        set => SetValue(FooterBrushProperty, value);
    }
        
    /// <summary>
    /// Gets or sets the data for the footer of this control.
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }
    
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's footer.
    /// </summary>
    public DataTemplate FooterTemplate
    {
        get => (DataTemplate)GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }
    
    public DataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
        
    /// <summary>
    /// Gets or sets the template used to display the content of the control's <see cref="Input"/> connectors.
    /// </summary>
    public DataTemplate InputConnectorTemplate
    {
        get => (DataTemplate)GetValue(InputConnectorTemplateProperty);
        set => SetValue(InputConnectorTemplateProperty, value);
    }
        
    /// <summary>
    /// Gets or sets the template used to display the content of the control's <see cref="Output"/> connectors.
    /// </summary>
    public DataTemplate OutputConnectorTemplate
    {
        get => (DataTemplate)GetValue(OutputConnectorTemplateProperty);
        set => SetValue(OutputConnectorTemplateProperty, value);
    }
        
    /// <summary>
    /// Gets or sets the data for the input <see cref="Connector"/>s of this control.
    /// </summary>
    public IEnumerable Input
    {
        get => (IEnumerable)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }
        
    /// <summary>
    /// Gets or sets the data for the output <see cref="Connector"/>s of this control.
    /// </summary>
    public IEnumerable Output
    {
        get => (IEnumerable)GetValue(OutputProperty);
        set => SetValue(OutputProperty, value);
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="Footer"/> is <see langword="null" />.
    /// </summary>
    public bool HasFooter => GetValue(FooterProperty)!=null;
    public bool HasHeader => GetValue(HeaderProperty)!=null;
    public Node()
    {
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased+= OnPointerReleased;
        
        // PointerPressedEvent.AddClassHandler<Node>(OnPointerPressed);
        //
        // PointerMovedEvent.AddClassHandler<Node>(OnPointerMoved);
        // PointerReleasedEvent.AddClassHandler<Node>(OnPointerReleased);
        //
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };
        _timer.Tick += OnTimerTick;
    }
    
     /// <summary>
    /// 记录上一次鼠标位置
    /// </summary>
    private Point lastMousePosition;

    /// <summary>
    /// 用于平滑更新坐标的计时器
    /// </summary>
    private DispatcherTimer _timer;

    /// <summary>
    /// 标记是否先启动了拖动
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// 需要更新的坐标点
    /// </summary>
    private Point _targetPosition;
    private void OnTimerTick(object? sender, EventArgs e)
    {
       
        
        ((NodeViewModelBase)DataContext).Location=new Point((_targetPosition.X + _startOffsetX),_targetPosition.Y+_startOffsetY);
       
        
    }
    private double _startOffsetX;
    private double _startOffsetY;
    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        this.GetVisualParent().ZIndex = 2;
        
        var visualParent = this.GetVisualParent();
        var parent = visualParent.GetVisualParent().GetVisualChildren();
        foreach (var visual in parent)
        {
            ((Node)visual.GetVisualChildren().First()).IsSelected = false;

        }
        this.IsSelected = true;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        // 启动拖动
        isDragging = true;
        // 记录当前坐标
        var relativeTo = ((Visual)this.GetLogicalParent()).GetVisualParent();
        lastMousePosition = e.GetPosition((Visual)relativeTo);
        _targetPosition = new Point(0,0);
        // Debug.WriteLine($"记录当前坐标X:{lastMousePosition.X} Y:{lastMousePosition.Y}");
        _startOffsetX = ((NodeViewModelBase)DataContext).Location.X;
        _startOffsetY = ((NodeViewModelBase)DataContext).Location.Y;
        e.Handled = true;
        // 启动计时器
        _timer.Start();
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        var visualParent = this.GetVisualParent();
        var parent = visualParent.GetVisualParent().GetVisualChildren();
        foreach (var visual in parent)
        {
            visual.ZIndex = 0;
        }
        visualParent.ZIndex = 1;
        if (!isDragging) return;
        // 停止拖动
        isDragging = false;
        e.Handled = true;
        // 停止计时器
        _timer.Stop();
        
        // var currentPoint = e.GetCurrentPoint(this);
       //  Debug.WriteLine($"停止拖动坐标X:{OffsetX} Y:{OffsetY}");
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(((Visual)this.GetLogicalParent()).GetVisualParent()).Properties.IsLeftButtonPressed) return;
        
        // 如果没有启动拖动，则不执行
        if (!isDragging) return;

        var currentMousePosition = e.GetPosition(((Visual)this.GetLogicalParent()).GetVisualParent());
        var offset = currentMousePosition - lastMousePosition;
        
        //lastMousePosition = e.GetPosition(this);
        // 记录当前坐标
        _targetPosition = new Point(offset.X,
            offset.Y );
        
        
    }
}