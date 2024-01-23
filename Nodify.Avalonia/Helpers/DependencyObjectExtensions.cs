

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Nodify.Avalonia.Helpers
{
    internal static class DependencyObjectExtensions
    {
        public static T? GetParentOfType<T>(this Control child)
            where T : Control
        {
            Visual? current = child;

            do
            {
                current = current.GetVisualParent();
                if (current == default)
                {
                    return default;
                }

            } while (current is not T);

            return (T)current;
        }
        public static T? GetChildOfType<T>(this Control control,string name)
            where T : Control
        {
            if (control.Name == name)
                return (T)control;

            foreach (var child in control.GetVisualChildren())
            {
                var foundChild = child as Control;
                if (foundChild != null && foundChild.Name == name)
                    return (T)foundChild;
                else
                {
                    var result = (T)(foundChild).GetChildOfType<Control>(name);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
        
        
    }
}
