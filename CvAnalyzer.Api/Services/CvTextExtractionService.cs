using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace CvAnalyzer.Api.Services;

public class CvTextExtractionService
{
    public string ExtractText(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => ExtractFromPdf(fileStream),
            ".docx" => ExtractFromDocx(fileStream),
            _ => throw new NotSupportedException($"File type '{extension}' is not supported. Please upload a PDF or DOCX file.")
        };
    }

    private string ExtractFromPdf(Stream fileStream)
    {
        var sb = new StringBuilder();
        using var document = PdfDocument.Open(fileStream);
        foreach (var page in document.GetPages())
        {
            sb.AppendLine(page.Text);
        }
        return sb.ToString();
    }

    private string ExtractFromDocx(Stream fileStream)
    {
        var sb = new StringBuilder();
        using var wordDoc = WordprocessingDocument.Open(fileStream, false);
        var body = wordDoc.MainDocumentPart?.Document?.Body;
        if (body != null)
        {
            foreach (var paragraph in body.Elements<Paragraph>())
            {
                sb.AppendLine(paragraph.InnerText);
            }
        }
        return sb.ToString();
    }
}