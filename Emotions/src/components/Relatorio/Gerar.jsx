import React, { useState } from "react";
import { APIService } from "../../http-common";
import "./Gerar.css";
import Navbar from "../NavBarLogin/NavBarLogin"; // Importando a Navbar

const GerarRelatorio = () => {
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [reportName, setReportName] = useState("");
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchRecords = async () => {
    setLoading(true);
    setError(null);

    try {
      const params = {
        startDate: startDate ? new Date(startDate).toISOString() : null,
        endDate: endDate ? new Date(endDate).toISOString() : null,
      };

      const response = await APIService.Axios().get("Home/GetAllDocuments", { params });

      if (response.status === 200) {
        setRecords(response.data);
      } else {
        throw new Error("Erro ao buscar registros.");
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const gerarRelatorio = () => {
    if (!reportName) {
      alert("Por favor, insira um nome para o relatório.");
      return;
    }

    // Aqui você pode integrar com outra API para salvar ou gerar o relatório
    console.log("Relatório Gerado:", { reportName, records });
    alert("Relatório gerado com sucesso!");
  };

  return (
    <>
      <Navbar /> {/* Adicionando a Navbar */}
      <div className="generate-report-page">
        <h1>Gerar Relatório</h1>
        <div className="form-container">
          <label htmlFor="report-name">Nome do Relatório:</label>
          <input
            type="text"
            id="report-name"
            value={reportName}
            onChange={(e) => setReportName(e.target.value)}
            placeholder="Digite o nome do relatório"
          />

          <div className="date-range-container">
            <div className="date-field">
              <label htmlFor="start-date">Data Início:</label>
              <input
                type="date"
                id="start-date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
              />
            </div>

            <div className="date-field">
              <label htmlFor="end-date">Data Fim:</label>
              <input
                type="date"
                id="end-date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
              />
            </div>

            <button onClick={fetchRecords} className="fetch-records-button">
              Buscar Registros
            </button>
          </div>
        </div>

        {loading && <p>Carregando registros...</p>}
        {error && <p className="error-message">Erro: {error}</p>}

        {records.length > 0 && (
          <div className="records-table-container">
            <h2>Registros Selecionados</h2>
            <table className="records-table">
              <thead>
                <tr>
                  <th>Data</th>
                  <th>Emoji</th>
                  <th>Título</th>
                  <th>Emoções</th>
                </tr>
              </thead>
              <tbody>
                {records.map((record, index) => (
                  <tr key={record.id || index}>
                    <td>
                  { 
                     record.data.toLocaleString()
              }
                </td>
                <td>{record.Emoji}</td>
                <td>{record.titulo}</td>
                    <td>
                      {Array.isArray(record.emocao)
                        ? record.emocao.join(", ")
                        : typeof record.emocao === "string"
                        ? record.emocao
                        : "N/A"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {records.length > 0 && (
          <button onClick={gerarRelatorio} className="generate-report-button">
            Gerar Relatório
          </button>
        )}
      </div>
    </>
  );
};

export default GerarRelatorio;
