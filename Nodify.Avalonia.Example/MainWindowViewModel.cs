using System.Collections.ObjectModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Nodify.Avalonia.Controls;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Example;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<NodeViewModelBase> nodes = new();
    [ObservableProperty]
    private ObservableCollection<ConnectionViewModelBase> connections=new ();
    public MainWindowViewModel()
    {
        var input1 = new ConnectorViewModelBase()
        {
            Title = "AS 1"
        };
        var output1 = new ConnectorViewModelBase()
        {
            Title = "B 1"
        };
        Connections.Add(new ConnectionViewModelBase(output1,input1));
        Nodes  =new(){
                new NodeViewModelBase()
                {
                    Location = new Point(100, 100),
                    Title = "Node 1",
                    Input = new ObservableCollection<ConnectorViewModelBase>
                    {
                        input1,
                       
                    },
                    Output = new ObservableCollection<ConnectorViewModelBase>
                    {
                       
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
                        output1,
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
    }

    
}