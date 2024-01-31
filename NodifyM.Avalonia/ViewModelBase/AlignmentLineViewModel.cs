using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class AlignmentLineViewModel : ObservableObject
{
    [ObservableProperty] private Point _start;
    [ObservableProperty] private Point _end;
    [ObservableProperty] private bool _isVisible=true;

    public AlignmentLineViewModel()
    {
        
    }
    public AlignmentLineViewModel(Point start, Point end)
    {
        Start = start;
        End = end;
        
    }
}