using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
using System.Runtime.InteropServices;

namespace ScoreViewer.Views
{
    public partial class PdfView : UserControl
    {
        public event EventHandler GotoMenu;

        private Avalonia.Controls.Image _pdfImage;
        private Button _backButton;
        private Button _nextPageButton;
        private Button _prevPageButton;
        private Button _openButton;
        private Button _exitButton;
        private int _currentPageIndex = 0;
        private PdfDocument _pdfDocument;
        PdfManager _pdfManager;
        SecondaryWindow _secondWindow;

        private Avalonia.Point initialTouchPosition;
        private double gestureSensitivity = 50; // Adjust the sensitivity as needed
        public PdfView()
        {
            InitializeComponent();
            _pdfImage = this.FindControl<Avalonia.Controls.Image>("PdfImage");
            _backButton = this.FindControl<Button>("BackButton");
            _nextPageButton = this.FindControl<Button>("NextPageButton");
            _prevPageButton = this.FindControl<Button>("PrevPageButton");
            _openButton = this.FindControl<Button>("OpenButton");
            _exitButton = this.FindControl<Button>("ExitButton");

            //_pdfManager = new PdfManager("C:\\images.pdf");


            _openButton.Click += (sender, e) =>
            {
                OpenPdf();
            };
        }
        public void Init(PdfManager pdfManager, MainWindow window, SecondaryWindow secondWindow)
        {
            _pdfManager = pdfManager;
            _secondWindow = secondWindow;
            _backButton.Click += (sender, e) =>
            {
                _pdfManager.Dispose();
                secondWindow.Hide();
                GotoMenu?.Invoke(this, EventArgs.Empty);
            };
            _nextPageButton.Click += (sender, e) =>
            {
                _pdfManager.NextPage();
                ShowPage();
            };

            _prevPageButton.Click += (sender, e) =>
            {
                _pdfManager.PrevPage();
                ShowPage();
            };
            ShowPage();
        }
        private void OpenPdf()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            if (commandLineArgs.Length > 1)
            {
                string filePath = commandLineArgs[1];
                _pdfManager = new PdfManager(filePath);
            }
        }
        public void ShowPage()
        {
            using (var stream = _pdfManager.RenderCurrentPage())
            {
                _pdfImage.Source = new Avalonia.Media.Imaging.Bitmap(stream);
                _secondWindow.UpdatePage();
            }
        }

        private void Window_InputEvents(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                _pdfManager.NextPage();
                ShowPage();
            }
            else if (e.Key == Key.Left)
            {
                _pdfManager.PrevPage();
                ShowPage();
            }
        }
        private void Window_PointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            var currentPosition = e.GetCurrentPoint(this).Position;
            var horizontalDelta = currentPosition.X - initialTouchPosition.X;

            if (horizontalDelta < -gestureSensitivity)
            {
                _pdfManager.NextPage();
                ShowPage();
            }
            else if (horizontalDelta > gestureSensitivity)
            {
                _pdfManager.PrevPage();
                ShowPage();
            }
        }

        private void Window_TouchStarted(object sender, Avalonia.Input.PointerEventArgs e)
        {
            initialTouchPosition = e.GetPosition(this);
        }
    }
}
