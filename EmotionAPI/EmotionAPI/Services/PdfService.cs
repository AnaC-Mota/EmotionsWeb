using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using System.IO;

public class PdfService
{
    public byte[] GerarPdf(string relatorioTexto)
    {
        using (var ms = new MemoryStream())
        {
            // Criar o PDF
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Criar uma fonte em negrito (Helvetica-Bold)
            PdfFont boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");

            // Adiciona o título em negrito
            document.Add(new Paragraph("Relatório Gerado")
                .SetFont(boldFont)  // Define a fonte em negrito
                .SetFontSize(16));

            // Adiciona o texto do relatório
            document.Add(new Paragraph(relatorioTexto));

            document.Close();
            return ms.ToArray();  // Retorna o PDF como byte array
        }
    }
}
