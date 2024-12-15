import React from "react";
import "./ModalHistorico.css";

const ModalHistorico = ({ isOpen, onRequestClose, record }) => {
  if (!isOpen) return null;

  const handleOverlayClick = (e) => {
    if (e.target.className === "modal-overlay") {
      onRequestClose();
    }
  };

  return (
    <div className="modal-overlay" onClick={handleOverlayClick}>
      <div className="modal-content">
        <button className="close-button" onClick={onRequestClose}>
          &times;
        </button>
        <h2>Detalhes da Emoção</h2>
        <p><strong>Data:</strong> {record.data}</p>
        <p><strong>Emoji:</strong> {record.Emoji || "N/A"}</p>
        <p><strong>Título:</strong> {record.titulo}</p>
        <p>
          <strong>Emoções:</strong>{" "}
          {Array.isArray(record.emocao)
            ? record.emocao.join(", ")
            : typeof record.emocao === "string"
            ? record.emocao
            : "N/A"}
        </p>
        <p><strong>Descrição:</strong> {record.descricao}</p>
        {/* Adicione mais campos se necessário */}
      </div>
    </div>
  );
};

export default ModalHistorico;
