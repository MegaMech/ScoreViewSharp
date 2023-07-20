using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using ScoreViewer.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;
using static SkiaSharp.HarfBuzz.SKShaper;


namespace ScoreViewer.Views
{
    public partial class MenuView : UserControl
    {
        private Button _exitButton;
        private TextBlock _textBlock;
        private TextBlock _error;

        public ICommand ItemClickedCommand { get; }

        public MenuView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            _exitButton = this.FindControl<Button>("ExitButton");
            _error = this.FindControl<TextBlock>("ErrorMessage");
            _error!.IsVisible = false;

            _exitButton.Click += (sender, e) =>
            {
                // Close the application
                System.Environment.Exit(1);
            };
        }

        public void setErrorMessage(string message) {
            _error.Text = message;
            _error.IsVisible = true;
        }

        public void clearErrorMessage() {
            _error.Text = string.Empty;
            _error.IsVisible = false;
        }

        private void TreeItemClickedHandler(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var viewModel = button.DataContext as Node;
            if (viewModel != null)
            {
                viewModel.NodeCommand.Execute(null);
            }
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            //if (DataContext is MenuViewModel viewModel)
            //{
            //    foreach (Node node in viewModel.Nodes)
            //    {
            //        var treeViewItem = (TreeViewItem)OneDriveTreeView.ContainerFromItem(node);
            //        if (treeViewItem != null)
            //        {
            //            treeViewItem.IsExpanded = true;
            //        }
            //    }
            //}
        }


    }
}
