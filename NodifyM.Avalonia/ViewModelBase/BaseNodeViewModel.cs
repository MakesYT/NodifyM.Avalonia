using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class BaseNodeViewModel : ObservableObject
{
    [ObservableProperty] private Point _location;
}