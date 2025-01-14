using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Image;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;

public class PdfService
{
    public byte[] GerarPdf(string relatorioTexto, string titulo = "Relatório Gerado")
    {
        try
        {
            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                document.Add(new Paragraph(titulo)
                    .SetFont(boldFont)
                    .SetFontSize(16)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetMarginBottom(20));

                document.Add(new Paragraph(relatorioTexto)
                    .SetFont(regularFont)
                    .SetFontSize(12)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.JUSTIFIED)
                    .SetMarginBottom(20));

                document.Close();
                return ms.ToArray();
            }
        }
        catch (PdfException pdfEx)
        {
            Console.WriteLine($"PDF Error: {pdfEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            throw;
        }
    }

    public string GerarPDFComImagem(string caminhoImagem)
    {
        try
        {
            // Caminho para salvar o PDF na pasta pública
            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
            if (!Directory.Exists(pdfFolder))
                Directory.CreateDirectory(pdfFolder);

            string pdfFileName = $"grafico_emocoes_{Guid.NewGuid()}.pdf";
            string caminhoPdf = Path.Combine(pdfFolder, pdfFileName);

            using (var writer = new PdfWriter(caminhoPdf))
            {
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                document.Add(new Paragraph("Relatório com Gráfico de Emoções")
                    .SetFont(boldFont)
                    .SetFontSize(16)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Usa o caminho físico da imagem
                string caminhoFisicoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoImagem.TrimStart('/'));
                ImageData imageData = ImageDataFactory.Create(caminhoFisicoImagem);
                Image img = new Image(imageData).SetAutoScale(true);
                document.Add(img);

                document.Close();
            }

            // Retorna o caminho público do PDF
            return $"/files/{pdfFileName}";
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao gerar o PDF.", ex);
        }
    }


}
