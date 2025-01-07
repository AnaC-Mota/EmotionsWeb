using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.IO;
using System.Collections.Generic;
using EmotionAPI.DTOs;

public class GoogleSheetsService
{
    private static string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private static string ApplicationName = "Google Sheets API .NET";
    private static string SpreadsheetId = "15nIVOuYvBiy7HpokMKZXPo8z07hIi9FIBcC4_I9IoaY";  // Substitua pelo seu ID da planilha
    private static string SheetName = "Dados";  // Nome da aba da planilha

    public static SheetsService GetSheetsService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("./relatorio.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(Scopes);
        }

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        return service;
    }

    public static void WriteToSheet(List<IList<object>> values)
    {
        var service = GetSheetsService();

        // Limpa os dados existentes na planilha
        ClearSheet();

        // Define os valores a serem escritos
        var valueRange = new ValueRange { Values = values };

        // Define a célula de início como A1
        var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, $"{SheetName}!A1");
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        // Executa a solicitação de escrita
        updateRequest.Execute();
    }


    // Método para converter os dados do Firestore para o formato do Google Sheets
    public static List<IList<object>> ConvertToSheetFormat(List<Dictionary<string, object>> firestoreEntries)
    {
        var sheetData = new List<IList<object>>();

        // Adiciona o cabeçalho (se necessário)
        sheetData.Add(new List<object> { "ID", "Data", "Emoção", "Descrição", "Emoji", "Título" });

        // Adiciona os dados de cada entrada
        foreach (var entry in firestoreEntries)
        {
            var row = new List<object>
        {
            Guid.NewGuid().ToString(),  // ID gerado automaticamente (caso necessário)
            DateTime.Now.ToString("dd/MM/yyyy"),  // Data atual como data de registro
            entry.GetValueOrDefault("emocao",""),
            entry.GetValueOrDefault("descricao", ""),
            entry.GetValueOrDefault("Emoji", ""),
            entry.GetValueOrDefault("titulo", "")
        };

            sheetData.Add(row);
        }

        return sheetData;
    }

    public static void ClearSheet()
    {
        var service = GetSheetsService();

        // Define o intervalo a ser limpo (todas as células da aba)
        var range = $"{SheetName}!A:Z"; // Ajuste conforme o número de colunas usadas
        var clearRequest = new ClearValuesRequest();

        // Envia a solicitação para limpar os dados
        var request = service.Spreadsheets.Values.Clear(clearRequest, SpreadsheetId, range);
        request.Execute();
    }

}
