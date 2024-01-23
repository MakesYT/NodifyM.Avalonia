using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Nodify.Avalonia.Helpers;

namespace Nodify.Avalonia.Controls;

public class TestLine : Control
{
    public static readonly AvaloniaProperty<Point> SourceProperty =
        AvaloniaProperty.Register<TestLine, Point>(nameof(Source), BoxValue.Point);

    public static readonly AvaloniaProperty<Point> TargetProperty =
        AvaloniaProperty.Register<TestLine, Point>(nameof(Target), BoxValue.Point);

    public Point Source
    {
        get => (Point)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the end point of this connection.
    /// </summary>
    public Point Target
    {
        get => (Point)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public TestLine()
    {
        TargetProperty.Changed.AddClassHandler<TestLine>((o, e) =>
        {
            o.InvalidateVisual();
        });
    }
    public override void Render(DrawingContext context)
    {
        // 创建一个新的画笔
        var pen = new Pen(Brushes.Black, 2);

        // 创建一个新的贝塞尔曲线
        var bezierSegment = new QuadraticBezierSegment()
        {
            Point1 = Source, // 控制点
            Point2 = Target  // 结束点
        };
        Debug.WriteLine( $"BezierSegment.Point1 = {Source.ToString()}, BezierSegment.Point2 = {Target.ToString()}");

        // 创建一个新的路径
        var pathFigure = new PathFigure()
        {
            StartPoint = Source, // 开始点
            Segments = { bezierSegment }
        };

        // 创建一个新的几何图形
        var geometry = new PathGeometry()
        {
            Figures = { pathFigure }
        };

        // 使用画笔和几何图形绘制曲线
        context.DrawGeometry(Brushes.Transparent, pen, geometry);
    }
}