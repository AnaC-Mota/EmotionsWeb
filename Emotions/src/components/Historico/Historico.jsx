import { useEffect, useState } from "react";
import ModalHistorico from "../ModalHistorico/ModalHistorico";
import "./Historico.css";
import { APIService } from "../../http-common";

const Historico = () => {
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");


  // Função para buscar registros
  const fetchRecords = async () => {
    try {
      const params = {
        startDate: startDate ? new Date(startDate) : null,
        endDate: endDate ? new Date(endDate): null
      };
  
      const response = await APIService.Axios().post("Home/GetAllDocuments", { ...params });
  
      if (response.status === 200) {
        setRecords(response.data);
      } else {
        throw new Error("Erro ao buscar os registros.");
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };
  
  useEffect(() => {
    fetchRecords();
  }, [startDate, endDate]);

  const openModal = (record) => {
    setSelectedRecord(record);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setSelectedRecord(null);
    setIsModalOpen(false);
  };

  if (loading) {
    return <p>Carregando registros...</p>;
  }

  if (error) {
    return <p>Erro: {error}</p>;
  }

  return (
    <div className="history-container">
      <h1>Histórico de Emoções</h1>
      <div className="date-filter">
        <label htmlFor="start-date">Data Início:</label>
        <input
          type="date"
          id="start-date"
          value={startDate}
          onChange={(e) => setStartDate(e.target.value)}
        />
        
        <label htmlFor="end-date">Data Fim:</label>
        <input
          type="date"
          id="end-date"
          value={endDate}
          onChange={(e) => setEndDate(e.target.value)}
        />
        
        <button onClick={fetchRecords}>Filtrar</button>
      </div>

      {records.length === 0 ? (
        <p>Nenhum registro encontrado para o usuário autenticado.</p>
      ) : (
        <table className="history-table">
          <thead>
            <tr>
              <th>Data</th>
              <th>Emoji</th>
              <th>Título</th>
              <th>Emoções</th>
              <th>Ação</th>
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
                <td>
                  <button
                    className="details-button"
                    onClick={() => openModal(record)}
                  >
                    Detalhes
                  </button>
                </td>
              </tr>
            ))}
          </tbody>


        </table>
      )}

      {selectedRecord && (
        <ModalHistorico
          isOpen={isModalOpen}
          onRequestClose={closeModal}
          record={selectedRecord}
        />
      )}
    </div>
  );
};

export default Historico;
