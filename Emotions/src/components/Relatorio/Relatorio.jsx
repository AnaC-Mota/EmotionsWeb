import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { APIService } from "../../http-common";
import "./Relatorio.css";


const Relatorio = () => {
    const navigate = useNavigate();
    const [reports, setReports] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);


    const fetchReport = async () => {
      setLoading(true)
      try{
        const response = await APIService.Axios().get("Relatorios")
        if(response.status == 200){
          setReports(response.data)
        }
      }catch(error){
        setError(error.message)
      }
      setLoading(false)
    }

    const Open = async (url) => {
      window.open(url, "_blank")
    } 
  
    useEffect(() => {
      fetchReport()
      console.log(reports)
    }, [])


  return (
    <>
      <div className="history-container">
        <h1>Relatório de Emoções</h1>
        <div className='button'>
          <button onClick={() => navigate("/gerar-relatorio")} className="create-report-button">
            Criar +
          </button> 
        </div>
      </div>
      {reports.length > 0 && (
          <div className="reports-table-container">
            <h2>Relatórios</h2>
            <table className="reports-table">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Data Inicio Registos</th>
                  <th>Data final registos</th>
                  <th>Data criação</th>
                </tr>
              </thead>
              <tbody>
                {reports.map((report, index) => (
                  <tr key={index} onClick={()=> {Open(report.relatorio)}}>
                    <td>{report.nome}</td>
                    <td>{report.data_inicio}</td>
                    <td>{report.data_fim}</td>
                    <td>{report.data_reg}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
    </>
  )
}

export default Relatorio
