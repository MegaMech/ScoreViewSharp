using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Input;

namespace ScoreViewer.ViewModels;

public class Node
{
    public ObservableCollection<Node> SubNodes { get; private set; }
    public string Title { get; }
    public bool IsFile { get; }
    public ICommand NodeCommand { get; init; }
    public ICommand NodeCommandClicked { get; init; }

    public ICommand ItemClickedCommand { get; set; }

    public string PdfPath { get; }

    // For Nodes with no subNodes
    public Node(string path, string title, ICommand nodeCommand, bool isFile = false)
    {
        Title = title;
        IsFile = isFile;
        SubNodes = new ObservableCollection<Node>();
        NodeCommand = new RelayCommand(
            execute: () => OnClick(),
            canExecute: () => true
        );
        PdfPath = path;

    }
    // For Nodes with subNodes
    public Node(string path, string title, ObservableCollection<Node> subNodes, ICommand nodeCommand, bool isFile = false)
    {
        Title = title;
        SubNodes = subNodes ?? new ObservableCollection<Node>();
        IsFile = isFile;
        NodeCommand = new RelayCommand(
            execute: () => OnClick(),
            canExecute: () => true
        );
        PdfPath = path;
    }

    public event Action<string>? PdfFileSelected;

    public void OnClick()
    {
        if (IsFile)
        {
            Debug.Print("CLICKED!");
            PdfFileSelected?.Invoke(PdfPath);
        }
    }
}

public partial class MenuViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Node> _nodes;
    public ObservableCollection<Node> Nodes
    {
        get => _nodes;
        set
        {
            if (_nodes != value)
            {
                _nodes = value;
                OnPropertyChanged(nameof(Nodes));
            }
        }
    }

    public event Action<string>? PdfFileSelected;

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ICommand NodeClickedCommand { get; set; }

    public MenuViewModel()
    {

        NodeClickedCommand = ReactiveCommand.Create<Node>(NodeClicked);
        // Get OneDrive Files
        Nodes = new ObservableCollection<Node>();

        string userProfilePath = Environment.GetEnvironmentVariable("UserProfile");
        string rootDirectoryPath = Path.Combine(userProfilePath, "OneDrive", "Scores");

        try
        {
            Node rootNode = LoadDirectory(rootDirectoryPath);
            Nodes.Add(rootNode);
        }
        catch (Exception ex)
        {
            Debug.Print("Error while loading OneDrive: ", ex);
        }
    }
    private void NodeClicked(Node node)
    {
        // Implement what happens when a node is clicked.
        Debug.WriteLine($"Node {node} was clicked.");
    }

    private Node LoadDirectory(string directoryPath)
    {
        string directoryName = Path.GetFileName(directoryPath);
        var directoryNode = new Node(null, directoryName, NodeClickedCommand);

        string[] subDirectoryPaths = Directory.GetDirectories(directoryPath);
        foreach (string subDirectoryPath in subDirectoryPaths)
        {
            var subDirectoryNode = LoadDirectory(subDirectoryPath);
            directoryNode.SubNodes.Add(subDirectoryNode);
        }

        string[] filePaths = Directory.GetFiles(directoryPath, "*.pdf");
        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Debug.Print(filePath);
            var fileNode = new Node(filePath, fileName, NodeClickedCommand, true); // Set IsFile to true for file nodes
            fileNode.PdfFileSelected += path => PdfFileSelected?.Invoke(filePath);
            directoryNode.SubNodes.Add(fileNode);
        }

        return directoryNode;
    }

}
