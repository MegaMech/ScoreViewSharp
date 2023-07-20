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
    public MainViewModel _viewModel;
    public MenuView _menuView;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        //_menuView = new MenuView();
        //Content = _menuView;

        _viewModel = new MainViewModel();
        DataContext = _viewModel.MenuViewModel;

        if (Content is MenuView)
        {
            _menuView = (MenuView)Content;
        } else { throw new Exception("Content type not MenuView"); }

        _viewModel.MenuViewModel.PdfFileSelected += OnPdfFilSelected;


        this.AddHandler(KeyDownEvent, Window_InputEvents, handledEventsToo: true);
        this.AddHandler(PointerReleasedEvent, Window_PointerReleased, handledEventsToo: true);
    }
    private void Window_InputEvents(object sender, KeyEventArgs e)
    {
    }

    private void Window_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        // Handle PointerReleased event here
    }
    public void OnGotoMenu(object sender, EventArgs e)
    {
        try
        {
            DataContext = _viewModel.MenuViewModel;
            Content = _menuView;
            this.WindowState = WindowState.Normal;
        }
        catch
        {
            throw new Exception("Error: The menu was null when attempting to switch back to it.");
        }
    }

    private PdfManager _pdfManager;
    private SecondaryWindow _secondWindow;

    public void OnPdfFilSelected(string path)
    {
        // Switching to PDF view has three steps
        // 1) Create the PdfManager
        // 2) Create the second window
        // 3) Create the PdfView

        try {
            _menuView.clearErrorMessage();
            _pdfManager = new PdfManager(path);
        } catch (PdfManagerException e) {
            _menuView.setErrorMessage(e.Message);
            return;
        }

        // Skip re-creating the second window if it already exists.
        if (_secondWindow != null)
        {
            _secondWindow.UpdatePdfManagerRef(_pdfManager);
            _secondWindow.Show();
        }
        else
        {
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
        }

        this.WindowState = WindowState.Maximized;

        // Finally launch the PDF view
        var pdfView = new PdfView(_pdfManager, this, _secondWindow);
        this.Content = pdfView;

        // Subscribe to the GotoMenu event.
        pdfView.GotoMenu += OnGotoMenu;
    }
}