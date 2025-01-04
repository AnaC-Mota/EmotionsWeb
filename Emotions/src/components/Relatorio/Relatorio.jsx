import React from 'react'
import { useNavigate } from "react-router-dom";
import "./Relatorio.css";


const Relatorio = () => {
    const navigate = useNavigate();
  return (
    <div className="history-container">
      <h1>Relatório de Emoções</h1>
      <div className='button'>
      <button onClick={() => navigate("/gerar-relatorio")} className="create-report-button">
        Criar +
      </button> 
      </div>
    </div>
  )
}

export default Relatorio
