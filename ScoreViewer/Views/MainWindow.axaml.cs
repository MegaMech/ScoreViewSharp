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
    //private MenuView _menuView;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        //_menuView = new MenuView();
        //Content = _menuView;

        _viewModel = new MainViewModel();
        //DataContext = _viewModel;
        //ViewContent = new MenuView();

        //MenuView menuView = new MenuView();
        //menuView.DataContext = new MenuViewModel();

        //ContentControl contentControl = this.FindControl<ContentControl>("ContentView");

        //contentControl.Content = menuView;

        //Content = new MenuView();


        _viewModel.MenuViewModel.PdfFileSelected += OnPdfFilSelected;
    }

    public void OnGotoMenu(object sender, EventArgs e)
    {

        //MenuViewModel menuViewModel = menuView.DataContext as MenuViewModel;
        //if (menuViewModel != null)
        //{
        //    // Use the menuViewModel instance here
        //}
        try
        {
            // MainViewModel
            //MenuView menuView = new MenuView();
            //DataContext = _viewModel.MenuViewModel;
            //this.Content = menuView;
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

        _pdfManager = new PdfManager(path);

        // Skip re-creating the second window if it already exists.
        if (_secondWindow != null)
        {
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

        // Finally launch the PDF view
        var pdfView = new PdfView();
        pdfView.Init(_pdfManager, this, _secondWindow);
        this.Content = pdfView;

        pdfView.GotoMenu += OnGotoMenu;
    }
}