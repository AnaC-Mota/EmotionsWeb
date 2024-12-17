import React, { useEffect, useState } from "react";
import ModalHistorico from "../ModalHistorico/ModalHistorico";
import "./Historico.css";
import { APIService } from "../../http-common";

const Historico = () => {
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [filteredRecords, setFilteredRecords] = useState([]);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");


  // Função para buscar registros
  const fetchRecords = async () => {
    try {
      const params = {
        startDate: startDate,
        endDate: endDate
      };
  
      const response = await APIService.Axios().get("Home/GetAllDocuments", { params });
  
      if (response.status === 200) {
        console.log(response.data);
        setRecords(response.data);
        setFilteredRecords(response.data);  // Inicialmente, exibe todos os registros
      } else {
        throw new Error("Erro ao buscar os registros.");
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const filterRecordsByDate = () => {
    let filtered = records;

    if (startDate) {
      filtered = filtered.filter((record) => new Date(record.data) >= new Date(startDate));
    }

    if (endDate) {
      filtered = filtered.filter((record) => new Date(record.data) <= new Date(endDate));
    }

    setFilteredRecords(filtered);
  };

  useEffect(() => {
    fetchRecords();
  }, []);
  
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
        
        <button onClick={filterRecordsByDate}>Filtrar</button>
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
