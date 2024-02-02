using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;
using NodifyM.Avalonia.ViewModelBase;

namespace NodifyM.Avalonia.Controls;

public class BaseNode : ContentControl
{
    public static readonly AvaloniaProperty<Point> LocationProperty =
        AvaloniaProperty.Register<BaseNode, Point>(nameof(Location));
    public static readonly RoutedEvent LocationChangedEvent = RoutedEvent.Register<NodeLocationEventArgs>(nameof(LocationChanged), RoutingStrategies.Bubble, typeof(BaseNode));
    public static readonly AvaloniaProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<BaseNode, bool>(nameof(IsSelected));
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    public event NodeLocationEventHandler LocationChanged
    {
        add => AddHandler(LocationChangedEvent, value);
        remove => RemoveHandler(LocationChangedEvent, value);
    }
    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    public BaseNode()
    {
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
       
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _editor = this.GetParentOfType<NodifyEditor>();
        _editor.NodifyAutoPanning += NodifyAutoPanningEvent;
    }

    private void NodifyAutoPanningEvent(object sender, NodifyAutoPanningEventArgs e)
    {
        if (e.Node != this)
        {
            return;
        }
        RaiseEvent(new NodeLocationEventArgs(((BaseNodeViewModel)DataContext).Location,this,LocationChangedEvent));

    }


    private NodifyEditor _editor;
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

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        e.GetCurrentPoint(this).Pointer.Capture(this);
        var visualParent = this.GetVisualParent();
        var parent = visualParent.GetVisualParent().GetVisualChildren();
        foreach (var visual in parent)
        {
            visual.ZIndex = 0;
            var first = visual.GetVisualChildren().First();
            if (first is BaseNode baseNode)
            {
                baseNode.IsSelected = false;
            }
        }
        visualParent.ZIndex = 1;
        this.IsSelected = true;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        // 启动拖动
        isDragging = true;
        // 记录当前坐标
        var relativeTo = ((Visual)this.GetLogicalParent()).GetVisualParent();
        lastMousePosition = e.GetPosition((Visual)relativeTo);

        // Debug.WriteLine($"记录当前坐标X:{lastMousePosition.X} Y:{lastMousePosition.Y}");
        _startOffsetX = ((BaseNodeViewModel)DataContext).Location.X;
        _startOffsetY = ((BaseNodeViewModel)DataContext).Location.Y;
        e.Handled = true;
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {

        
        if (!isDragging) return;
        // 停止拖动
        isDragging = false;
        e.Handled = true;
        // 停止计时器
        _editor.ClearAlignmentLine();

        // var currentPoint = e.GetCurrentPoint(this);
        //  Debug.WriteLine($"停止拖动坐标X:{OffsetX} Y:{OffsetY}");
        RaiseEvent(new NodeLocationEventArgs(((BaseNodeViewModel)DataContext).Location,this,LocationChangedEvent,true));
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        RaiseEvent(new NodeLocationEventArgs(((BaseNodeViewModel)DataContext).Location,this,LocationChangedEvent,true));
        _editor.ClearAlignmentLine();
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(((Visual)this.GetLogicalParent()).GetVisualParent()).Properties
                .IsLeftButtonPressed) return;

        // 如果没有启动拖动，则不执行
        if (!isDragging) return;

        var currentMousePosition = e.GetPosition(((Visual)this.GetLogicalParent()).GetVisualParent());
        var offset = currentMousePosition - lastMousePosition;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            _editor.ClearAlignmentLine();
            ((BaseNodeViewModel)DataContext).Location = new Point((offset.X + _startOffsetX), offset.Y + _startOffsetY);
        }
        else
            ((BaseNodeViewModel)DataContext).Location = _editor.TryAlignNode(this,
                new Point((offset.X + _startOffsetX), offset.Y + _startOffsetY));

        RaiseEvent(new NodeLocationEventArgs(((BaseNodeViewModel)DataContext).Location,this,LocationChangedEvent));
    }
}