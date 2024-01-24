using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nodify.Avalonia.Controls;

namespace Nodify.Avalonia.ViewModelBase;

public partial class PendingConnectionViewModelBase : ObservableObject
{
     private readonly NodifyEditorViewModelBase _editor;

    [ObservableProperty] private object? _previewTarget;
    [ObservableProperty] private string _previewText;
    [ObservableProperty] private ConnectorViewModelBase _source;

    public PendingConnectionViewModelBase(NodifyEditorViewModelBase editor)
    {
        _editor = editor;
    }

    partial void OnPreviewTargetChanged(object? value)
    {
        var canConnect = value != null;
        switch (value)
        {
            case ConnectorViewModelBase con:
            {
                if (con == Source)
                {
                    PreviewText = $"不能自己连接自己";
                    break;
                }
                
                

                PreviewText = "连接";

                break;
            }
            default:
                PreviewText = $"丢弃连接";
                break;
        }
    }

    [RelayCommand]
    public void Start(ConnectorViewModelBase item) => Source = item;

    [RelayCommand]
    public void Finish(ConnectorViewModelBase? target)
    {
        if (target == null)
        {
            return;
        }

        if (target == Source)
        {
            return;
        }

        _editor.Connect(target, Source);
    }
}