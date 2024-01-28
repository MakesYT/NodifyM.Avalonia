# NodifyM.Avalonia
[![NuGet](https://img.shields.io/nuget/v/NodifyM.Avalonia?style=for-the-badge&logo=nuget&label=release)](https://www.nuget.org/packages/NodifyM.Avalonia/)
[![NuGet](https://img.shields.io/nuget/dt/NodifyM.Avalonia?label=downloads&style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/NodifyM.Avalonia)
[![License](https://img.shields.io/github/license/miroiu/nodify?style=for-the-badge)](https://github.com/miroiu/nodify/blob/master/LICENSE)

A collection of controls for node based editors designed for MVVM.
## About
This project is a refactoring of [Nodify](https://github.com/miroiu/nodify) on the Avalonia platform and is not a 1:1 replica of Nodify, but they have many similarities.
![image](https://raw.githubusercontent.com/MakesYT/NodifyM.Avalonia/master/assets/Kitopia1706354432972.png)

## Notice
This project is still in its early stages and lacks many events compared to **Nodify**, but it is already available

## Example
#### Here is a complete example of control usage
xaml:
```xaml
<controls:NodifyEditor
            Background="Transparent"
            ItemsSource="{Binding Nodes }"
            Connections="{Binding Connections}"
            PendingConnection="{Binding PendingConnection}"
            DisconnectConnectorCommand="{Binding DisconnectConnectorCommand}">
            <controls:NodifyEditor.Resources>
                <converters:FlowToDirectionConverter x:Key="FlowToDirectionConverter" />
            </controls:NodifyEditor.Resources>
            <controls:NodifyEditor.GridLineTemplate>
                <DataTemplate>
                    <controls:LargeGridLine Width="{Binding $parent[controls:NodifyEditor].Bounds.Width}"
                                            OffsetX="{Binding $parent[controls:NodifyEditor].OffsetX}"
                                            OffsetY="{Binding $parent[controls:NodifyEditor].OffsetY}"
                                            Zoom="{Binding $parent[controls:NodifyEditor].Zoom}"
                                            Height="{Binding $parent[controls:NodifyEditor].Bounds.Height}"
                                            Spacing="30"
                                            Thickness="0.5"
                                            Brush="LightGray"
                                            ZIndex="-2"/>
                </DataTemplate>
            </controls:NodifyEditor.GridLineTemplate>
            <controls:NodifyEditor.ConnectionTemplate>
                <DataTemplate DataType="{x:Type viewModelBase:ConnectionViewModelBase}">
                    <Grid>
                        <controls:CircuitConnection
                            Direction="{Binding Source.Flow,Converter={StaticResource FlowToDirectionConverter}}"
                            Source="{Binding Source.Anchor}" Focusable="True"
                            Target="{Binding Target.Anchor}">
                            <controls:CircuitConnection.Stroke>
                                <SolidColorBrush Color="Red"
                                                 Opacity="0.5" />
                            </controls:CircuitConnection.Stroke>
                        </controls:CircuitConnection>
                    </Grid>

                </DataTemplate>
            </controls:NodifyEditor.ConnectionTemplate>
            <controls:NodifyEditor.PendingConnectionTemplate>
                <DataTemplate DataType="{x:Type viewModelBase:PendingConnectionViewModelBase}">
                    <controls:PendingConnection
                        StartedCommand="{Binding StartCommand}"
                        CompletedCommand="{Binding FinishCommand}"
                        EnablePreview="True"
                        EnableSnapping="True"
                        Direction="{Binding Source.Flow,Converter={StaticResource FlowToDirectionConverter}}"
                        PreviewTarget="{Binding PreviewTarget, Mode=OneWayToSource}">
                        <controls:PendingConnection.Stroke>
                            <SolidColorBrush Color="Red"
                                             Opacity="0.5" />

                        </controls:PendingConnection.Stroke>
                        <controls:PendingConnection.Background>
                            <SolidColorBrush Color="DodgerBlue"
                                             Opacity="0.8" />
                        </controls:PendingConnection.Background>
                        <TextBlock Text="{Binding PreviewText}" />


                    </controls:PendingConnection>

                </DataTemplate>
            </controls:NodifyEditor.PendingConnectionTemplate>
            <controls:NodifyEditor.ItemTemplate>
                <DataTemplate DataType="viewModelBase:NodeViewModelBase">
                    <controls:Node x:Name="Node"
                                   Input="{Binding Input}"
                                   Header="{Binding Title}"
                                   VerticalAlignment="Center"
                                   Output="{Binding Output}">
                        <controls:Node.Styles>
                            <Style Selector="controls|Node[IsSelected=False]:pointerover">
                                <Setter Property="BorderBrush" Value="AliceBlue"></Setter>
                                <Setter Property="BorderThickness" Value="2"></Setter>
                            </Style>
                        </controls:Node.Styles>
                        <controls:Node.InputConnectorTemplate>
                            <DataTemplate DataType="{x:Type viewModelBase:ConnectorViewModelBase}">
                                <controls:NodeInput
                                    x:Name="NodeInput"
                                    VerticalAlignment="Center"
                                    IsConnected="{Binding IsConnected}"
                                    Anchor="{Binding Anchor, Mode=OneWayToSource}">
                                    <controls:NodeInput.Header>
                                        <StackPanel Orientation="Horizontal" VerticalAlignment="Center"
                                                    HorizontalAlignment="Right">

                                            <TextBlock VerticalAlignment="Center" x:Name="textBlock"
                                                       Text="{Binding Title}" />
                                        </StackPanel>
                                    </controls:NodeInput.Header>
                                    <controls:NodeInput.BorderBrush>
                                        <SolidColorBrush
                                            Color="CornflowerBlue"
                                            Opacity="0.5" />
                                    </controls:NodeInput.BorderBrush>
                                </controls:NodeInput>
                            </DataTemplate>
                        </controls:Node.InputConnectorTemplate>

                        <controls:Node.OutputConnectorTemplate>
                            <DataTemplate DataType="{x:Type viewModelBase:ConnectorViewModelBase}">
                                <controls:NodeOutput
                                    x:Name="NodeOutput"
                                    VerticalAlignment="Center"
                                    IsConnected="{Binding IsConnected}"
                                    Anchor="{Binding Anchor, Mode=OneWayToSource}">
                                    <controls:NodeOutput.Header>
                                        <StackPanel Orientation="Horizontal" VerticalAlignment="Center"
                                                    HorizontalAlignment="Right">

                                            <TextBlock VerticalAlignment="Center" x:Name="textBlock"
                                                       Text="{Binding Title}" />
                                        </StackPanel>
                                    </controls:NodeOutput.Header>
                                    <controls:NodeOutput.BorderBrush>
                                        <SolidColorBrush
                                            Color="CornflowerBlue"
                                            Opacity="0.5" />
                                    </controls:NodeOutput.BorderBrush>
                                </controls:NodeOutput>
                            </DataTemplate>
                        </controls:Node.OutputConnectorTemplate>
                    </controls:Node>
                </DataTemplate>
            </controls:NodifyEditor.ItemTemplate>

        </controls:NodifyEditor>
```
ViewModel cs:  
Notice: Bind ViewModel by yourself
```csharp
public partial class MainWindowViewModel : NodifyEditorViewModelBase
{
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
        Connections.Add(new ConnectionViewModelBase(output1, input1));
        Nodes = new()
        {
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
```
You can override Connect and Disconnect methods
```csharp
public override void Connect(ConnectorViewModelBase source, ConnectorViewModelBase target)
{
    base.Connect(source, target);
}

public override void DisconnectConnector(ConnectorViewModelBase connector)
{
    base.DisconnectConnector(connector);
}
```