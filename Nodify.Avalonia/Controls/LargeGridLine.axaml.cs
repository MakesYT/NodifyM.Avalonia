using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Nodify.Avalonia.Helpers;

namespace Nodify.Avalonia.Controls;

public class LargeGridLine : TemplatedControl
{
    public static readonly AvaloniaProperty<double> OffsetXProperty=AvaloniaProperty.Register<LargeGridLine,double>(nameof(OffsetX));
    public static readonly AvaloniaProperty<double> OffsetYProperty=AvaloniaProperty.Register<LargeGridLine,double>(nameof(OffsetY));
    public static readonly AvaloniaProperty<double> ZoomProperty=AvaloniaProperty.Register<LargeGridLine,double>(nameof(Zoom));
    
    public double OffsetX
    {
        get { return (double)GetValue(OffsetXProperty); }
        set { SetValue(OffsetXProperty, value); }
    }
    public double OffsetY
    {
        get { return (double)GetValue(OffsetYProperty); }
        set { SetValue(OffsetYProperty, value); }
    }
    public double Zoom
    {
        get { return (double)GetValue(ZoomProperty); }
        set { SetValue(ZoomProperty, value); }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        OffsetXProperty.Changed.AddClassHandler<LargeGridLine>((_,_) =>
        {
            InvalidateVisual();
        } );
        OffsetYProperty.Changed.AddClassHandler<LargeGridLine>((_, _) =>
        {
            InvalidateVisual();
        } );
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var pen = new Pen(Brushes.LightGray, 0.5);
        double step = 20;
        // Draw horizontal lines
        var offsetY = Math.Abs(OffsetY/Zoom);
        var offsetX = Math.Abs(OffsetX/Zoom);
        for (double y = OffsetY%20; y < this.Bounds.Height; y += step)
        {
            context.DrawLine(pen, new Point(-offsetX, y), new Point(this.Bounds.Width, y));
        }

        // Draw vertical lines
        
        for (double x = OffsetX%20; x < this.Bounds.Width; x += step)
        {
            context.DrawLine(pen, new Point(x, -offsetY), new Point(x, this.Bounds.Height));
        }
    }
}