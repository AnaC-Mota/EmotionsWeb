using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GraficoService
{
    public string GerarGraficoDeEmocoes(Dictionary<string, int> contagemEmocoes)
    {
        try
        {
            // Cria o objeto Plot
            var plt = new ScottPlot.Plot();

            // Obtém os dados do gráfico
            string[] emocoes = contagemEmocoes.Keys.ToArray();
            double[] valores = contagemEmocoes.Values.Select(v => (double)v).ToArray();

            // Adiciona o gráfico de barras
            plt.AddBar(values: valores);
            plt.XTicks(positions: Enumerable.Range(0, emocoes.Length).Select(i => (double)i).ToArray(), labels: emocoes);

            // Título e rótulos
            plt.Title("Frequência das Emoções");
            plt.YLabel("Quantidade");
            plt.XLabel("Emoções");

            // Caminho para salvar a imagem na pasta pública
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            string nomeImagem = $"grafico_emocoes_{Guid.NewGuid()}.png";
            string caminhoImagem = Path.Combine(imagesFolder, nomeImagem);

            // Salva o gráfico como PNG
            plt.SaveFig(caminhoImagem);

            // Retorna o caminho público
            return $"/images/{nomeImagem}";
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao gerar o gráfico de emoções.", ex);
        }
    }

    public string GerarGraficoDeScore(List<ResultScores> datas)
    {
        var plt = new ScottPlot.Plot();

        // Converte datas para valores numéricos (OADate)
        double[] xs = datas.Select(d => d.data.ToOADate()).ToArray();
        double[] ys = datas.Select(d => d.scores).ToArray();

        // Adiciona o gráfico de linha
        plt.AddScatter(xs, ys);

        // Formata o eixo X como datas
        plt.XAxis.DateTimeFormat(true);

        plt.Title("Evolução do Score Emocional");
        plt.YLabel("Score");
        plt.XLabel("Data");

        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string nomeImagem = $"score_emocional_{Guid.NewGuid()}.png";
        string caminho = Path.Combine(folder, nomeImagem);
        plt.SaveFig(caminho);
        return $"/images/{nomeImagem}";
    }




}
