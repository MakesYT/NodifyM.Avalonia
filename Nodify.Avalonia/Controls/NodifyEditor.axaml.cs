using System.Collections;
using System.Collections.ObjectModel;
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
    public NodifyEditor()
    {

        
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
    
}