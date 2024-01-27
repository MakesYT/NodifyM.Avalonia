using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace NodifyM.Avalonia.Helpers
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
            if (control.Name == name&& control is T)
                return (T)control;

            foreach (var child in control.GetVisualChildren())
            {
                var foundChild = child as Control;
                if (foundChild != null && foundChild.Name == name&& foundChild is T)
                    return (T)foundChild;
                else
                {
                    var result = (T)((foundChild).GetChildOfType<Control>(name));
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
        public static T? GetVisualAt<T>(this Control control,Point anchor)
            where T : Control
        {
            var visualAt = control.GetVisualAt(anchor);
            if (visualAt == null)
                return null;
            else
                return ((Control)visualAt).GetParentOfType<T>();
        }
        
    }
}