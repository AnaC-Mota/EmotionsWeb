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

    // Método para escrever dados na planilha do Google Sheets
    public static void WriteToSheet(List<IList<object>> values)
    {
        var service = GetSheetsService();

        var valueRange = new ValueRange();
        valueRange.Values = values;

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, $"{SheetName}!A1");
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        var appendResponse = appendRequest.Execute();
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

}
