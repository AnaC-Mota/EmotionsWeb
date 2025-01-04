using EmotionAPI.DTOs;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirestoreService
{
    private FirestoreDb _firestoreDb;

    public FirestoreService()
    {
        _firestoreDb = FirestoreDb.Create("217645884982"); // Substitua pelo seu ID do projeto Firebase
    }

    public async Task<List<DadosRegistros>> GetEntriesAsync()
    {
        var entriesRef = _firestoreDb.Collection("entries");
        var snapshot = await entriesRef.GetSnapshotAsync();

        var registros = new List<DadosRegistros>();

        foreach (var document in snapshot.Documents)
        {
            var registro = document.ConvertTo<DadosRegistros>();
            registros.Add(registro);
        }

        return registros;
    }
}
