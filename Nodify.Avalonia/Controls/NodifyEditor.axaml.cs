using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using Nodify.Avalonia.Helpers;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Controls;

public class NodifyEditor : ItemsControl
{
   
#if DEBUG
    public ObservableCollection<NodeViewModelBase> Nodes { get; set; } =
        new(){
            new NodeViewModelBase()
            {
                Location = new Point(100, 100),
                Title = "Node 1",
                Input = new ObservableCollection<ConnectorViewModelBase>
                {
                    new ConnectorViewModelBase()
                    {
                        Title = "Input 1"
                    },
                    new ConnectorViewModelBase()
                    {
                        Title = "Input 2"
                    }
                },
                Output = new ObservableCollection<ConnectorViewModelBase>
                {
                    new ConnectorViewModelBase()
                    {
                        Title = "Output 1"
                    },
                    new ConnectorViewModelBase()
                    {
                        Title = "Output 2"
                    }
                }
            },
            new NodeViewModelBase()
            {
                Title = "Node 2",
                Input = new ObservableCollection<ConnectorViewModelBase>
                {
                    new ConnectorViewModelBase()
                    {
                        Title = "Input 1"
                    },
                    new ConnectorViewModelBase()
                    {
                        Title = "Input 2"
                    }
                },
                Output = new ObservableCollection<ConnectorViewModelBase>
                {
                    new ConnectorViewModelBase()
                    {
                        Title = "Output 1"
                    },
                    new ConnectorViewModelBase()
                    {
                        Title = "Output 2"
                    }
                }
            }
        };
#endif
    public NodifyEditor()
    {
#if DEBUG
        
        this.ItemsSource = Nodes;
#endif
        
       
    }
}