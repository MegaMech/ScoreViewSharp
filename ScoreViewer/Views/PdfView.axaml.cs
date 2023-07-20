using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ReactiveUI;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using PdfiumViewer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using ExCSS;

namespace ScoreViewer.Views
{
    public partial class PdfView : UserControl
    {
        public event EventHandler GotoMenu;

        private Avalonia.Controls.Image ?_pdfImage;
        private StackPanel _pageArea;
        private StackPanel _userInterface;
        private Button ?_backButton;
        private TextBlock ?_pageNumber;
        //private Button ?_prevPageButton;
        private Button ?_inkpenButton;
        private Button ?_exitButton;
        private int _currentPageIndex = 0;
        private PdfDocument _pdfDocument;
        PdfManager _pdfManager;
        SecondaryWindow _secondWindow;

        private Avalonia.Point initialTouchPosition;
        private double gestureSensitivity = 50; // Adjust the sensitivity as needed
        public PdfView(PdfManager pdfManager, MainWindow window, SecondaryWindow secondWindow)
        {
            InitializeComponent();
            _pdfImage = this.FindControl<Avalonia.Controls.Image>("PdfImage");
            _pageArea = this.FindControl<StackPanel>("PageArea");
            _userInterface = this.FindControl<StackPanel>("UserInterface");
            _backButton = this.FindControl<Button>("BackButton");
            _pageNumber = this.FindControl<TextBlock>("PageNumber");
            //_prevPageButton = this.FindControl<Button>("PrevPageButton");
            _inkpenButton = this.FindControl<Button>("InkpenButton");
            _exitButton = this.FindControl<Button>("ExitButton");

            _inkpenButton.Click += (sender, e) =>
            {
               // OpenPdf();
            };
            _pdfManager = pdfManager;
            _secondWindow = secondWindow;
            _backButton.Click += (sender, e) =>
            {
                _pdfManager.Dispose();
                
                secondWindow.Hide();
                GotoMenu?.Invoke(this, EventArgs.Empty);
            };
            ////_nextPageButton.Click += (sender, e) =>
            //{
            //    _pdfManager.NextPage();
            //    ShowPage();
            //};

            //_prevPageButton.Click += (sender, e) =>
            //{
            //    _pdfManager.PrevPage();
            //    ShowPage();
            //};
            this.GetObservable(KeyDownEvent).Subscribe(Window_InputEvents);
            //this.GetObservable(PointerPressedEvent).Subscribe(Window_PointerPressed);
            _pageNumber.Text = "1/"+_pdfManager.GetPageCount();
            ShowPage();
            _pageArea.PointerPressed += OnPointerPressed;
            _pageArea.PointerReleased += OnPointerReleased;
            _pageArea.PointerMoved += OnPointerMoved;
        }
   

        private void Window_InputEvents(KeyEventArgs e)
        {
            if (!_userInterface.IsVisible)
            {
                if (e.Key == Key.Right)
                {
                    // Handle right arrow key press
                    UpdatePageNumber(_pdfManager.NextPage());
                    ShowPage();
                }
                else if (e.Key == Key.Left)
                {
                    // Handle left arrow key press
                    UpdatePageNumber(_pdfManager.PrevPage());
                    ShowPage();
                }
            }
        }

        private Avalonia.Point initialPosition;
        private bool hasSwitchedPage = false;
        private bool isGestureInProgress = false;
        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            initialPosition = e.GetPosition(null);
        }
        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (!isGestureInProgress)
            {
                Debug.Print("InitX: ",initialPosition.X.ToString());
                Debug.Print("InitY: ",initialPosition.Y.ToString());

                var currentPosition = e.GetPosition(null);
                var horizontalDeviation = Math.Abs(currentPosition.X - initialPosition.X);
                var verticalDeviation = Math.Abs(currentPosition.Y - initialPosition.Y);
                var deviationThreshold = 50; // Define your deviation threshold here
                Debug.Print("DevX: ", verticalDeviation.ToString());
                Debug.Print("DevY: ",verticalDeviation.ToString());
                if (horizontalDeviation <= deviationThreshold && verticalDeviation <= deviationThreshold)
                {
                    _userInterface.IsVisible = !_userInterface.IsVisible;
                }
            }
                hasSwitchedPage = false;
                isGestureInProgress = false;
        }
        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            if (!_userInterface.IsVisible)
            {
                if (!hasSwitchedPage)
                {
                    var currentPosition = e.GetPosition(null);
                    var horizontalDelta = currentPosition.X - initialPosition.X;

                    if (horizontalDelta > 0)
                    {
                        if (!isGestureInProgress)
                        {
                            isGestureInProgress = true;
                            hasSwitchedPage = true;
                            UpdatePageNumber(_pdfManager.NextPage());
                            ShowPage();
                        }
                    }
                    else if (horizontalDelta < 0)
                    {
                        if (!isGestureInProgress)
                        {
                            isGestureInProgress = true;
                            hasSwitchedPage = true;
                            UpdatePageNumber(_pdfManager.PrevPage());
                            ShowPage();
                        }
                    }
                }
            }
        }
        private void OnPointerCaptureLost(object sender, PointerCaptureLostEventArgs e)
        {
            isGestureInProgress = false;
            hasSwitchedPage = false;
        }
        // Originally used for manually opening a pdf file.
        //private void OpenPdf()
        //{
        //    string[] commandLineArgs = Environment.GetCommandLineArgs();

        //    if (commandLineArgs.Length > 1)
        //    {
        //        string filePath = commandLineArgs[1];
        //        _pdfManager = new PdfManager(filePath);
        //    }
        //}
        public void ShowPage()
        {
            using (var stream = _pdfManager.RenderCurrentPage())
            {
                var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);
                _pdfImage.Source = PostProcessing(bitmap);
                _secondWindow.UpdatePage();
            }
        }

        public Avalonia.Media.Imaging.Bitmap PostProcessing(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            return bitmap;
        }
        private void Window_PointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            //var currentPosition = e.GetCurrentPoint(this).Position;
            //var horizontalDelta = currentPosition.X - initialTouchPosition.X;

            //if (horizontalDelta < -gestureSensitivity)
            //{
            //    UpdatePageNumber(_pdfManager.NextPage());
            //    ShowPage();
            //}
            //else if (horizontalDelta > gestureSensitivity)
            //{
            //    UpdatePageNumber(_pdfManager.PrevPage());
            //    ShowPage();
            //}
        }

        private void UpdatePageNumber(string text)
        {
            _pageNumber.Text = text;
        }

        private void Window_TouchStarted(object sender, Avalonia.Input.PointerEventArgs e)
        {
            initialTouchPosition = e.GetPosition(this);
        }
    }
}
