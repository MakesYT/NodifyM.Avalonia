using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.VisualTree;
using NodifyM.Avalonia.ViewModelBase;

namespace NodifyM.Avalonia.Controls;

public class Node : BaseNode
{
    public static readonly AvaloniaProperty<IBrush> ContentBrushProperty =
        AvaloniaProperty.Register<Node, IBrush>(nameof(ContentBrush));

    public static readonly AvaloniaProperty<IBrush> HeaderBrushProperty =
        AvaloniaProperty.Register<Node, IBrush>(nameof(HeaderBrush));

    public static readonly AvaloniaProperty<IBrush> FooterBrushProperty =
        AvaloniaProperty.Register<Node, IBrush>(nameof(FooterBrush));

    public static readonly AvaloniaProperty<object> FooterProperty =
        AvaloniaProperty.Register<Node, object>(nameof(Footer));

    public static readonly AvaloniaProperty<object> HeaderProperty =
        AvaloniaProperty.Register<Node, object>(nameof(Header));

    public static readonly AvaloniaProperty<IDataTemplate> FooterTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(FooterTemplate));

    public static readonly AvaloniaProperty<IDataTemplate> HeaderTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(HeaderTemplate));

    public static readonly AvaloniaProperty<IDataTemplate> InputConnectorTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(InputConnectorTemplate));

    protected internal static readonly AvaloniaProperty<bool> HasFooterProperty =
        AvaloniaProperty.RegisterDirect<Node, bool>(nameof(HasFooter), o => o.HasFooter);

    protected internal static readonly AvaloniaProperty<bool> HasHeaderProperty =
        AvaloniaProperty.RegisterDirect<Node, bool>(nameof(HasHeader), o => o.HasHeader);

    public static readonly StyledProperty<IDataTemplate> OutputConnectorTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(OutputConnectorTemplate));

    public static readonly AvaloniaProperty<IEnumerable> InputProperty =
        AvaloniaProperty.Register<Node, IEnumerable>(nameof(Input));

    public static readonly AvaloniaProperty<IEnumerable> OutputProperty =
        AvaloniaProperty.Register<Node, IEnumerable>(nameof(Output));

   

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
    public IDataTemplate FooterTemplate
    {
        get => (DataTemplate)GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    public IDataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's <see cref="Input"/> connectors.
    /// </summary>
    public IDataTemplate InputConnectorTemplate
    {
        get => (DataTemplate)GetValue(InputConnectorTemplateProperty);
        set => SetValue(InputConnectorTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's <see cref="Output"/> connectors.
    /// </summary>
    public IDataTemplate OutputConnectorTemplate
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
    public bool HasFooter => GetValue(FooterProperty) != null;

    public bool HasHeader => GetValue(HeaderProperty) != null;

    public Node()
    {
    }
}