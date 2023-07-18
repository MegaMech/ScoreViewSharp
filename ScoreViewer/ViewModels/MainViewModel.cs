using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreViewer.ViewModels;
public partial class MainViewModel : ViewModelBase
{
    public MenuViewModel MenuViewModel { get; }
    //public ViewModelBase ViewContent { get; } = "MenuViewModel";

    public delegate void MainWindowFunctionDelegate();
    public MainWindowFunctionDelegate MainWindowFunction { get; set; }

    public MainViewModel()
    {
        MenuViewModel = new MenuViewModel();
        
        //ViewContent = MenuViewModel;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isMenuViewVisible = true;
    public bool IsMenuViewVisible
    {
        get => _isMenuViewVisible;
        set
        {
            if (_isMenuViewVisible != value)
            {
                _isMenuViewVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMenuViewVisible)));
            }
        }
    }

    private bool _isPdfViewVisible = false;
    public bool IsPdfViewVisible
    {
        get => _isPdfViewVisible;
        set
        {
            if (_isPdfViewVisible != value)
            {
                _isPdfViewVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPdfViewVisible)));
            }
        }
    }
}