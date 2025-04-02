import "./ModalHistorico.css";
import PropTypes from "prop-types";


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
      </div>
    </div>
  );
};

ModalHistorico.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onRequestClose: PropTypes.func.isRequired,
  record: PropTypes.shape({
    data: PropTypes.string,
    Emoji: PropTypes.string,
    titulo: PropTypes.string,
    emocao: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.string), PropTypes.string]),
    descricao: PropTypes.string,
  }).isRequired,
};

export default ModalHistorico;
