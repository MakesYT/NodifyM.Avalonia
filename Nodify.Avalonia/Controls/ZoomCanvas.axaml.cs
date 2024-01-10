using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Nodify.Avalonia.Helpers;

namespace Nodify.Avalonia.Controls;

public class ZoomCanvas : Canvas
{
    private static readonly AvaloniaProperty<double> ZoomProperty = AvaloniaProperty.Register<Node, double>(nameof(Zoom),1d);
    public static readonly AvaloniaProperty<double> OffsetXProperty = AvaloniaProperty.Register<Node, double>(nameof(OffsetX),1d);
    public static readonly AvaloniaProperty<double> OffsetYProperty = AvaloniaProperty.Register<Node, double>(nameof(OffsetY),1d);
    
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

    public ZoomCanvas()
    {
        
        
    }

    public override void ApplyTemplate()
    {
        base.ApplyTemplate();
        ZoomProperty.Changed.AddClassHandler<ZoomCanvas>(OnZoomChanged);
        PointerWheelChangedEvent.AddClassHandler<ZoomCanvas>(OnPointerWheelChanged);
        ((Control)Parent).SizeChanged+=(OnSizeChanged);
        PointerPressedEvent.AddClassHandler<ZoomCanvas>(OnPointerPressed);
        PointerMovedEvent.AddClassHandler<ZoomCanvas>(OnPointerMoved);
        PointerReleasedEvent.AddClassHandler<ZoomCanvas>(OnPointerReleased);
        
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
    private void OnTimerTick(object sender, EventArgs e)
    {
       
        if (Math.Abs(OffsetX - _startOffsetX) != Math.Abs(_targetPosition.X) || Math.Abs(OffsetY - _startOffsetY) != Math.Abs(_targetPosition.Y))
        {
            // 更新坐标
            OffsetX = _targetPosition.X+_startOffsetX;
            OffsetY = _targetPosition.Y+_startOffsetY;
            //Debug.WriteLine($"更新坐标X:{OffsetX} Y:{OffsetY}");
            
        }
    }
    private double _startOffsetX;
    private double _startOffsetY;
    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        // 启动拖动
        isDragging = true;
        // 记录当前坐标
        lastMousePosition = e.GetPosition(this);
        _targetPosition = new Point(0,0);
        // Debug.WriteLine($"记录当前坐标X:{lastMousePosition.X} Y:{lastMousePosition.Y}");
        _startOffsetX = OffsetX;
        _startOffsetY = OffsetY;
        e.Handled = true;
        // 启动计时器
        _timer.Start();
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (!isDragging) return;
        // 停止拖动
        isDragging = false;
        e.Handled = true;
        // 停止计时器
        _timer.Stop();
        
        // var currentPoint = e.GetCurrentPoint(this);
         Debug.WriteLine($"停止拖动坐标X:{OffsetX} Y:{OffsetY}");
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        
        // 如果没有启动拖动，则不执行
        if (!isDragging) return;

        var currentMousePosition = e.GetPosition(this);
        var offset = currentMousePosition - lastMousePosition;
        
        //lastMousePosition = e.GetPosition(this);
        // 记录当前坐标
        _targetPosition = new Point(offset.X,
            offset.Y );
        
        
    }
    

    private double _initWeight;
    private double _initHeight;
    private void OnZoomChanged(ZoomCanvas arg1, AvaloniaPropertyChangedEventArgs arg2)
    {
        Width = _initWeight / arg2.GetNewValue<double>();
        Height = _initHeight /  arg2.GetNewValue<double>();
         
        
    }
    
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
    private void OnPointerWheelChanged(ZoomCanvas nodifyEditor, PointerWheelEventArgs pointerWheelEventArgs)
    {
        var position = pointerWheelEventArgs.GetPosition(this);
        
        var delta = pointerWheelEventArgs.Delta.Y;
        if (delta < 0)
        {
            _nowScale-=0.1d;
            if (_nowScale<=0.1d)
            {
                _nowScale=0.1d;
            }
            
           
        }
        else
        {
            _nowScale+=0.1d;
            if (_nowScale>=10d)
            {
                _nowScale=10d;
            }
           
           
        }
       
       Debug.WriteLine($"缩放比例:{_nowScale}");
        Debug.WriteLine( $"X:{OffsetX} Y:{OffsetY}");
        Debug.WriteLine( $"Pos:X:{position.X} Y:{position.Y}");
        OffsetX-= (position.X / (_nowScale - 1))/_nowScale;
        //OffsetY-= (position.Y / (_nowScale - 1))/_nowScale;
        Debug.WriteLine( $"X:{OffsetX} Y:{OffsetY}");
        
        Zoom = _nowScale;
        RenderTransform= new ScaleTransform(Zoom, Zoom);
       
        pointerWheelEventArgs.Handled = true;
    }
}