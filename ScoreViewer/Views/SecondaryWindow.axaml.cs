using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.Collections.Generic;

namespace ScoreViewer.Views;
public partial class SecondaryWindow : Window
{
    private Image _pdfImage;
    private PdfManager _pdfManager;
    public SecondaryWindow(PdfManager pdfManager)
    {
        InitializeComponent();
        _pdfManager = pdfManager;
        _pdfImage = this.FindControl<Avalonia.Controls.Image>("PdfImage2");

        this.GetObservable(KeyDownEvent).Subscribe(Window_InputEvents);
    }
    public void UpdatePage()
    {
        using (var stream = _pdfManager.RenderSecondaryPage())
        {
            if (stream == null)
            {
                _pdfImage.Source = null;
            } else
            {
                _pdfImage.Source = new Avalonia.Media.Imaging.Bitmap(stream);
            }
        }
    }
    public void UpdatePdfManagerRef(PdfManager pdfManager)
    {
        _pdfManager = pdfManager;
    }

    private void Window_InputEvents(KeyEventArgs e)
    {
        //if (!_userInterface.IsVisible)
        //{
        //    if (e.Key == Key.Right)
        //    {
        //        // Handle right arrow key press
        //        UpdatePageNumber(_pdfManager.NextPage());
        //        ShowPage();
        //    }
        //    else if (e.Key == Key.Left)
        //    {
        //        // Handle left arrow key press
        //        UpdatePageNumber(_pdfManager.PrevPage());
        //        ShowPage();
        //    }
        //}
    }
}
