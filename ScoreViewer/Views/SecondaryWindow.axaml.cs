using Avalonia.Controls;
using Avalonia.Platform;
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
}
