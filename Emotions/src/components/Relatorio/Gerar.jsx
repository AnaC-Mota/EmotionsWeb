import { useState } from "react";
import { APIService } from "../../http-common";
import "./Gerar.css";
import Navbar from "../NavBarLogin/NavBarLogin";

const GerarRelatorio = () => {
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [reportName, setReportName] = useState("");
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Função para truncar datas para YYYY-MM-DD
  /*const truncateToDate = (isoDate) => {
    const date = new Date(isoDate);
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  };*/

  // Buscar registros do backend
  const fetchRecords = async () => {
    setLoading(true);
    setError(null);

    try {
      // Validação de data
      if (startDate && endDate && new Date(startDate) > new Date(endDate)) {
        throw new Error("A data de início não pode ser maior que a data de fim.");
      }

      const params = {
        startDate: startDate ? new Date(startDate) : null,
        endDate: endDate ? new Date(endDate) : null,
      };

      // Buscar registros
      const response = await APIService.Axios().post("Home/GetAllDocuments", { ...params });
      console.log(response.data
      )
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



  // Gerar relatório
  const gerarGrafico = async () => { 
    const params = {
      startDate: startDate ? new Date(startDate) : null,
      endDate: endDate ? new Date(endDate) : null,
      title: reportName
    };
    setLoading(true); setError(null); 
    try { 
      const response = await APIService.Axios().post("Grafico", {
        ...params
      }); 
      if (response.status === 200) { 
        const { pdfUrl  } = response.data; 
        console.log("Gráfico gerado com sucesso:", pdfUrl ); 
        window.open(pdfUrl , "_blank");
      } else { 
        throw new Error("Erro ao gerar o gráfico em PDF."); } 
      } catch (err) { 
        setError(err.message); 
      } finally { 
        setLoading(false); 
      } 
    };
  return (
    <>
      <Navbar />
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

            <button
              onClick={fetchRecords}
              className="fetch-records-button"
              disabled={loading}
            >
              {loading ? "Buscando..." : "Buscar Registros"}
            </button>
          </div>
        </div>

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
                    <td>{new Date(record.data).toLocaleDateString()}</td>
                    <td>{record.Emoji}</td>
                    <td>{record.titulo}</td>
                    <td>
                      {Array.isArray(record.emocao)
                        ? record.emocao.join(", ")
                        : record.emocao || "N/A"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {records.length > 0 && (
          <button
            onClick={gerarGrafico}
            className="generate-report-button"
            disabled={loading}
          >
            {loading ? "Gerando..." : "Gerar Relatório"}
          </button>
        )}
      </div>
    </>
  );
};

export default GerarRelatorio;
