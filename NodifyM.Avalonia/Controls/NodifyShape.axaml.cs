

#nullable enable
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;

// Decompiled with JetBrains decompiler
// Type: Avalonia.Controls.Shapes.NodifyShape
// Assembly: Avalonia.Controls, Version=11.0.7.0, Culture=neutral, PublicKeyToken=c8d484a7012f9a8b
// MVID: 2B6AC9F3-0180-4F51-8F38-1CB4A1A36C4E
// Assembly location: C:\Users\13540\.nuget\packages\avalonia\11.0.7\ref\net6.0\Avalonia.Controls.dll
// XML documentation location: C:\Users\13540\.nuget\packages\avalonia\11.0.7\ref\net6.0\Avalonia.Controls.xml
namespace NodifyM.Avalonia.Controls
{
  /// <summary>
  /// Provides a base class for shape elements, such as <see cref="T:Avalonia.Controls.Shapes.Ellipse" />, <see cref="T:Avalonia.Controls.Shapes.Polygon" /> and <see cref="T:Avalonia.Controls.Shapes.Rectangle" />.
  /// </summary>
  public abstract class NodifyShape : Control
  {
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.Fill" /> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FillProperty = AvaloniaProperty.Register<NodifyShape, IBrush>(nameof (Fill));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.Stretch" /> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty = AvaloniaProperty.Register<NodifyShape, Stretch>(nameof (Stretch));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.Stroke" /> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> StrokeProperty = AvaloniaProperty.Register<NodifyShape, IBrush>(nameof (Stroke));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.StrokeDashArray" /> property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty = AvaloniaProperty.Register<NodifyShape, AvaloniaList<double>>(nameof (StrokeDashArray));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.StrokeDashOffset" /> property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeDashOffsetProperty = AvaloniaProperty.Register<NodifyShape, double>(nameof (StrokeDashOffset));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.StrokeThickness" /> property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty = AvaloniaProperty.Register<NodifyShape, double>(nameof (StrokeThickness));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.StrokeLineCap" /> property.
    /// </summary>
    public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty = AvaloniaProperty.Register<NodifyShape, PenLineCap>(nameof (StrokeLineCap));
    /// <summary>
    /// Defines the <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.StrokeJoin" /> property.
    /// </summary>
    public static readonly StyledProperty<PenLineJoin> StrokeJoinProperty = AvaloniaProperty.Register<NodifyShape, PenLineJoin>(nameof (StrokeJoin), PenLineJoin.Miter);
    private Matrix _transform = Matrix.Identity;
    private Geometry? _definingGeometry;
    private Geometry? _renderedGeometry;
    private IPen? _strokePen;

    /// <summary>
    /// Gets a value that represents the <see cref="T:Avalonia.Media.Geometry" /> of the shape.
    /// </summary>
    protected abstract Geometry DefiningGeometry { get; }

    /// <summary>
    /// Gets a value that represents the final rendered <see cref="T:Avalonia.Media.Geometry" /> of the shape.
    /// </summary>
    public Geometry? RenderedGeometry
    {
      get
      {
        if (this._renderedGeometry == null && this.DefiningGeometry != null)
        {
          if (this._transform == Matrix.Identity)
          {
            this._renderedGeometry = this.DefiningGeometry;
          }
          else
          {
            this._renderedGeometry = this.DefiningGeometry.Clone();
            this._renderedGeometry.Transform = this._renderedGeometry.Transform == null || this._renderedGeometry.Transform.Value == Matrix.Identity ? (Transform) new MatrixTransform(this._transform) : (Transform) new MatrixTransform(this._renderedGeometry.Transform.Value * this._transform);
          }
        }
        return this._renderedGeometry;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:Avalonia.Media.IBrush" /> that specifies how the shape's interior is painted.
    /// </summary>
    public IBrush? Fill
    {
      get => this.GetValue<IBrush>(NodifyShape.FillProperty);
      set => this.SetValue<IBrush>(NodifyShape.FillProperty, value);
    }

    /// <summary>
    /// Gets or sets a <see cref="P:NodifyM.Avalonia.Controls.NodifyShape.Stretch" /> enumeration value that describes how the shape fills its allocated space.
    /// </summary>
    public Stretch Stretch
    {
      get => this.GetValue<Stretch>(NodifyShape.StretchProperty);
      set => this.SetValue<Stretch>(NodifyShape.StretchProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="T:Avalonia.Media.IBrush" /> that specifies how the shape's outline is painted.
    /// </summary>
    public IBrush? Stroke
    {
      get => this.GetValue<IBrush>(NodifyShape.StrokeProperty);
      set => this.SetValue<IBrush>(NodifyShape.StrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets a collection of <see cref="T:System.Double" /> values that indicate the pattern of dashes and gaps that is used to outline shapes.
    /// </summary>
    public AvaloniaList<double>? StrokeDashArray
    {
      get => this.GetValue<AvaloniaList<double>>(NodifyShape.StrokeDashArrayProperty);
      set => this.SetValue<AvaloniaList<double>>(NodifyShape.StrokeDashArrayProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies the distance within the dash pattern where a dash begins.
    /// </summary>
    public double StrokeDashOffset
    {
      get => this.GetValue<double>(NodifyShape.StrokeDashOffsetProperty);
      set => this.SetValue<double>(NodifyShape.StrokeDashOffsetProperty, value);
    }

    /// <summary>Gets or sets the width of the shape outline.</summary>
    public double StrokeThickness
    {
      get => this.GetValue<double>(NodifyShape.StrokeThicknessProperty);
      set => this.SetValue<double>(NodifyShape.StrokeThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets a <see cref="T:Avalonia.Media.PenLineCap" /> enumeration value that describes the shape at the ends of a line.
    /// </summary>
    public PenLineCap StrokeLineCap
    {
      get => this.GetValue<PenLineCap>(NodifyShape.StrokeLineCapProperty);
      set => this.SetValue<PenLineCap>(NodifyShape.StrokeLineCapProperty, value);
    }

    /// <summary>
    /// Gets or sets a <see cref="T:Avalonia.Media.PenLineJoin" /> enumeration value that specifies the type of join that is used at the vertices of a NodifyShape.
    /// </summary>
    public PenLineJoin StrokeJoin
    {
      get => this.GetValue<PenLineJoin>(NodifyShape.StrokeJoinProperty);
      set => this.SetValue<PenLineJoin>(NodifyShape.StrokeJoinProperty, value);
    }

    public override void Render(DrawingContext context)
    {
      Geometry renderedGeometry = this.DefiningGeometry;
      if (renderedGeometry == null)
        return;
      context.DrawGeometry(this.Fill, this._strokePen, renderedGeometry);
    }

    /// <summary>Marks a property as affecting the shape's geometry.</summary>
    /// <param name="properties">The properties.</param>
    /// <remarks>
    /// After a call to this method in a control's static constructor, any change to the
    /// property will cause <see cref="M:NodifyM.Avalonia.Controls.NodifyShape.InvalidateGeometry" /> to be called on the element.
    /// </remarks>
    protected static void AffectsGeometry<TShape>(params AvaloniaProperty[] properties) where TShape : NodifyShape
    {
      foreach (AvaloniaProperty property in properties)
        property.Changed.Subscribe<AvaloniaPropertyChangedEventArgs>((Action<AvaloniaPropertyChangedEventArgs>) (e =>
        {
          if (!(e.Sender is TShape sender2))
            return;
          NodifyShape.AffectsGeometryInvalidate((NodifyShape) sender2, e);
        }));
    }

  

    /// <summary>Invalidates the geometry of this shape.</summary>
    protected void InvalidateGeometry()
    {
      this._renderedGeometry = (Geometry) null;
      this._definingGeometry = (Geometry) null;
      this.InvalidateMeasure();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
      base.OnPropertyChanged(change);
      if (change.Property == (AvaloniaProperty) NodifyShape.StrokeProperty || change.Property == (AvaloniaProperty) NodifyShape.StrokeThicknessProperty || change.Property == (AvaloniaProperty) NodifyShape.StrokeDashArrayProperty || change.Property == (AvaloniaProperty) NodifyShape.StrokeDashOffsetProperty || change.Property == (AvaloniaProperty) NodifyShape.StrokeLineCapProperty || change.Property == (AvaloniaProperty) NodifyShape.StrokeJoinProperty)
      {
        if (change.Property == (AvaloniaProperty) NodifyShape.StrokeProperty || change.Property == (AvaloniaProperty) NodifyShape.StrokeThicknessProperty)
          this.InvalidateMeasure();
        object?[]? parameters = [this._strokePen, this.Stroke, this.StrokeThickness, (IList<double>) this.StrokeDashArray, this.StrokeDashOffset, this.StrokeLineCap, this.StrokeJoin,10.0];


        var methodInfo = typeof(Pen).GetMethod("TryModifyOrCreate", BindingFlags.NonPublic|BindingFlags.Static);
        if ((bool)methodInfo.Invoke(null, parameters))
        {
          _strokePen = (IPen?)parameters[0];
          return;
        }
        _strokePen = (IPen?)parameters[0];
        this.InvalidateVisual();
      }
      else
      {
        if (!(change.Property == (AvaloniaProperty) NodifyShape.FillProperty))
          return;
        this.InvalidateVisual();
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      return this.DefiningGeometry == null ? new Size() : NodifyShape.CalculateSizeAndTransform(availableSize, this.DefiningGeometry.Bounds, this.Stretch).size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (this.DefiningGeometry == null)
        return new Size();
      Matrix transform = NodifyShape.CalculateSizeAndTransform(finalSize, this.DefiningGeometry.Bounds, this.Stretch).transform;
      if (this._transform != transform)
      {
        this._transform = transform;
        this._renderedGeometry = (Geometry) null;
      }
      return finalSize;
    }

    internal static (Size size, Matrix transform) CalculateSizeAndTransform(
      Size availableSize,
      Rect shapeBounds,
      Stretch Stretch)
    {
      Size size = new Size(shapeBounds.Right, shapeBounds.Bottom);
      Matrix matrix1 = Matrix.Identity;
      double width = availableSize.Width;
      double height = availableSize.Height;
      double num1 = 0.0;
      double num2 = 0.0;
      if (Stretch != Stretch.None)
      {
        size = shapeBounds.Size;
        matrix1 = Matrix.CreateTranslation(-(Vector) shapeBounds.Position);
      }
      if (double.IsInfinity(availableSize.Width))
        width = size.Width;
      if (double.IsInfinity(availableSize.Height))
        height = size.Height;
      if (shapeBounds.Width > 0.0)
        num1 = width / size.Width;
      if (shapeBounds.Height > 0.0)
        num2 = height / size.Height;
      if (double.IsInfinity(availableSize.Width))
        num1 = num2;
      if (double.IsInfinity(availableSize.Height))
        num2 = num1;
      switch (Stretch)
      {
        case Stretch.Fill:
          if (double.IsInfinity(availableSize.Width))
            num1 = 1.0;
          if (double.IsInfinity(availableSize.Height))
          {
            num2 = 1.0;
            break;
          }
          break;
        case Stretch.Uniform:
          num1 = num2 = Math.Min(num1, num2);
          break;
        case Stretch.UniformToFill:
          num1 = num2 = Math.Max(num1, num2);
          break;
        default:
          num1 = num2 = 1.0;
          break;
      }
      Matrix matrix2 = matrix1 * Matrix.CreateScale(num1, num2);
      return (new Size(size.Width * num1, size.Height * num2), matrix2);
    }

    private static void AffectsGeometryInvalidate(NodifyShape control, AvaloniaPropertyChangedEventArgs e)
    {
      if (e.Property == (AvaloniaProperty) Visual.BoundsProperty && ((Rect) e.OldValue).Size == ((Rect) e.NewValue).Size)
        return;
      control.InvalidateGeometry();
    }
  }
}
