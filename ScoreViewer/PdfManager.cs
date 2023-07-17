using PdfiumViewer;
using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using Avalonia;

public class PdfManager
{
    private PdfDocument _pdfDocument;
    private int _currentPageIndex = 0;

    public PdfManager(string filePath)
    {
        _pdfDocument = PdfDocument.Load(filePath);
    }

    public int PageCount => _pdfDocument.PageCount;

    public Stream RenderPage(int pageIndex)
    {
        if (_pdfDocument.PageCount > pageIndex && pageIndex >= 0)
        {
            var imageSize = _pdfDocument.PageSizes[pageIndex];
            var imageStream = new MemoryStream();
            using (var image = _pdfDocument.Render(pageIndex, (int)imageSize.Width, (int)imageSize.Height, true))
            {
                image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            imageStream.Seek(0, SeekOrigin.Begin);
            return imageStream;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex));
        }
    }

    public void NextPage()
    {
        if (_currentPageIndex < PageCount - 2)
        {
            _currentPageIndex += 2;
        }
    }
    public void PrevPage()
    {
        if (_currentPageIndex > 0)
        {
            _currentPageIndex -= 2;
        }
    }

    public Stream RenderCurrentPage()
    {
        return RenderPage(_currentPageIndex);
    }

    public Stream RenderSecondaryPage()
    {
        if (_currentPageIndex + 1 > PageCount - 1) { return null; }
        return RenderPage(_currentPageIndex + 1);
    }
    public void Dispose()
    {
        _pdfDocument.Dispose();
    }
}
