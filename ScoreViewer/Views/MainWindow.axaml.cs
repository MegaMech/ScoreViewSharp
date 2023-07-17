using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using PdfiumViewer;
using ScoreViewer.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace ScoreViewer.Views;

public partial class MainWindow : Window
{
    private MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        _viewModel.MenuViewModel.PdfFileSelected += OnPdfFilSelected;
    }

    private PdfManager _pdfManager;
    private SecondaryWindow _secondWindow;

    public void OnPdfFilSelected(string path)
    {
        // Switching to PDF view has three steps
        // 1) Create the PdfManager
        // 2) Create the second window
        // 3) Create the PdfView

        _pdfManager = new PdfManager(path);


        // Create secondary window
        var screens = Screens.All;
        _secondWindow = new SecondaryWindow(_pdfManager);

        if (screens.Count >= 2)
        {
            // Get the second screen
            var secondScreen = screens[1];
            _secondWindow.Position = secondScreen.WorkingArea.Position;
            _secondWindow.Width = secondScreen.WorkingArea.Width;
            _secondWindow.Height = secondScreen.WorkingArea.Height;
        }
        _secondWindow.WindowState = WindowState.Maximized;
        _secondWindow.Show();


        // Finally launch the PDF view
        var pdfView = new PdfView(_pdfManager, this, _secondWindow);
        this.Content = pdfView;
    }
}