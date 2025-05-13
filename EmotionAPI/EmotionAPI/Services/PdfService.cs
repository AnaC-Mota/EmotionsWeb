using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Image;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using Aspose.Pdf.Facades;
using iText.Layout.Properties;

public class PdfService
{
    public byte[] GerarPdfComTextoEImagem(string relatorioTexto, string caminhoImagem, string caminhoImagemEmocoes)
    {
        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);


        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        document.Add(new Paragraph("Relatório Emocional")
            .SetFont(boldFont)
            .SetFontSize(16)
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            .SetMarginBottom(20));

        document.Add(new Paragraph(relatorioTexto)
            .SetFont(regularFont)
            .SetFontSize(12)
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.JUSTIFIED)
            .SetMarginBottom(20));

        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

        if (!string.IsNullOrEmpty(caminhoImagem))
        {
            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoImagem.TrimStart('/'));
            var imageData = ImageDataFactory.Create(imgPath);
            var img = new Image(imageData).SetAutoScale(true);
            document.Add(img);
        }

        if(!string.IsNullOrEmpty(caminhoImagemEmocoes))
        {
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new Paragraph("Frequência das Emoções")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10));

            string imgPathEmocoes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoImagemEmocoes.TrimStart('/'));
            var imageDataEmocoes = ImageDataFactory.Create(imgPathEmocoes);
            var imgEmocoes = new Image(imageDataEmocoes).SetAutoScale(true);
            document.Add(imgEmocoes);
        }

        document.Close();
        return ms.ToArray();
    }



}
