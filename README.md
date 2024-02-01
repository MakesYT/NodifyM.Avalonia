# NodifyM.Avalonia
[![NuGet](https://img.shields.io/nuget/v/NodifyM.Avalonia?style=for-the-badge&logo=nuget&label=release)](https://www.nuget.org/packages/NodifyM.Avalonia/)
[![NuGet](https://img.shields.io/nuget/dt/NodifyM.Avalonia?label=downloads&style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/NodifyM.Avalonia)
[![License](https://img.shields.io/github/license/miroiu/nodify?style=for-the-badge)](https://github.com/miroiu/nodify/blob/master/LICENSE)

A collection of controls for node based editors designed for MVVM.
## About
This project is a refactoring of [Nodify](https://github.com/miroiu/nodify) on the Avalonia platform and is not a 1:1 replica of Nodify, but they have many similarities.
![image](https://raw.githubusercontent.com/MakesYT/NodifyM.Avalonia/master/assets/Kitopia1706713366453.png)

## Features
 - Designed from the start to work with **MVVM**
 - Built-in dark and light **themes**
 - **Selecting**， **zooming**， **panning**
 - Select， move, **_auto align_**, **auto panning** when close to edge and connect nodes
### What are the differences compared to Nodify
 - **Supports** 
   - auto align Node
   - display text on Connection
 - **Nonsupport**
   - Select multiple nodes
 - **Will be supported in the future**
   - none
## Usage
### NodifyEditor
 - `Press` and `Hold` -> Move the all show items
 -  Mouse wheel -> Zoom all show items
### Node
 - `Press` and `Hold` -> Move the Node
 - `Press Move` and `Hold Shift` -> Move the Node(without automatically align)
 - `Press` the Node -> Select the Node
### Connection
 - `Press` and `Hold` the Connector and move to another Connector -> Create a new connection
 - Hold `Alt` and `Click` Connection -> Remove Connection
 - `DoubleClick` Connection -> Split the connection in the double-click position
### PendingConnection
 - `Press` and `Hold` the Connector -> Show connection preview
### Connector
 - Hold `Alt` and `Click` Connector -> Remove all the Connections on the Connector

## Notice
This project is still in its early stages and lacks many events compared to **Nodify**, but it is already available

## Example
#### please see the [NodifyM.Avalonia.Example](https://github.com/MakesYT/NodifyM.Avalonia/tree/master/NodifyM.Avalonia.Example)
#### You can git clone the project and run `NodifyM.Avalonia.Example`

## Changelog
### 1.0.7
- Added Node **auto panning** when close to edge
### 1.0.6
- Fixed Node IsSelected property
- Fixed Node BorderBrush Style
- Added Node Alignment hint
### 1.0.5
 - Added the ability to temporarily without automatically align Node while holding Shift
 - Added the ability to display text on Connection
### 1.0.4
 - Add align Node configuration properties
 - Add Node automatic alignment
### 1.0.3
 - Added the connection SplitConnection and DisconnectConnection commands
 - Add CircuitConnection
 - Fixed default control color to dictionary color
 - Support to override the Connect and Disconnect from NodifyEditorViewModelBase method
 - Fix KnotNode Show
 - Remove some useless attributes