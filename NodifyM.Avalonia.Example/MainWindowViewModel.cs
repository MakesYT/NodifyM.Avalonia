using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nodify.Avalonia.Controls;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Example;

public partial class MainWindowViewModel : NodifyEditorViewModelBase{
    public MainWindowViewModel()
    {
        var input1 = new ConnectorViewModelBase()
        {
            Title = "AS 1",
            Flow = ConnectorViewModelBase.ConnectorFlow.Input
        };
        var output1 = new ConnectorViewModelBase()
        {
            Title = "B 1",
            Flow = ConnectorViewModelBase.ConnectorFlow.Output
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
                            Title = "Output 2",
                            Flow = ConnectorViewModelBase.ConnectorFlow.Output
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
                            Title = "Input 1",
                            Flow = ConnectorViewModelBase.ConnectorFlow.Input
                        },
                        new ConnectorViewModelBase()
                        {
                            Flow = ConnectorViewModelBase.ConnectorFlow.Input,
                            Title = "Input 2"
                        }
                    },
                    Output = new ObservableCollection<ConnectorViewModelBase>
                    {
                        output1,
                        new ConnectorViewModelBase()
                        {
                            Flow = ConnectorViewModelBase.ConnectorFlow.Output,
                            Title = "Output 1"
                        },
                        new ConnectorViewModelBase()
                        {
                            Flow = ConnectorViewModelBase.ConnectorFlow.Output,
                            Title = "Output 2"
                        }
                    }
                }
            };
        output1.IsConnected = true;
        input1.IsConnected = true;
    }

    
}