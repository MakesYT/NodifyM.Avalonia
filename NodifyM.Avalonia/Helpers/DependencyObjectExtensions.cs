using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace NodifyM.Avalonia.Helpers
{
    internal static class DependencyObjectExtensions
    {
        public static T? GetParentOfType<T>(this Control child,string? name=null)
            where T : Control
        {
            Visual? current = child;
            
            while (true)
            {
                
            
                current = current.GetVisualParent();
                if (current == default)
                {
                    return default;
                }

                if (current is T control)
                {
                    if (name!=null&&control.Name!=name)
                    {
                        continue;
                    }
                    return control;
                }
            } 

            
        }
        public static T? GetChildOfType<T>(this Control control,string? name=null)
            where T : Control
        {
            if ((name ==null||control.Name == name)&& control is T control1)
                return control1;

            foreach (var child in control.GetVisualChildren())
            {
                var foundChild = child as Control;
                if (foundChild != null && (name==null||foundChild.Name == name)&& foundChild is T child1)
                    return child1;
                else
                {
                    var result = ((foundChild).GetChildOfType<T>(name));
                    if (result != null)
                        return (T)result;
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