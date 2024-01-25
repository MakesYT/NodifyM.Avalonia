using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nodify.Avalonia.Controls;

namespace Nodify.Avalonia.ViewModelBase;


public partial class PendingConnectionViewModelBase(NodifyEditorViewModelBase editor) : ObservableObject
{
    private string[] _previewTargetNames = new string[] { "不能自己连接自己", "连接","丢弃连接" };
    
    [ObservableProperty] private object? _previewTarget;
    [ObservableProperty] private string? _previewText;
    [ObservableProperty] private ConnectorViewModelBase? _source;

    partial void OnPreviewTargetChanged(object? value)
    {
        switch (value)
        {
            case ConnectorViewModelBase con:
            {
                if (con == Source)
                {
                    PreviewText = _previewTargetNames[0];
                    break;
                }
                
                

                PreviewText = _previewTargetNames[1];

                break;
            }
            default:
                PreviewText = _previewTargetNames[2];
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

        editor.Connect(Source, target);
    }
}