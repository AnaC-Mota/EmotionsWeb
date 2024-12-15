import React from "react";

function ListaHistorico({ record }) {
  const handleDetails = () => {
    alert(`Detalhes do Registro:\nTítulo: ${record.title}\nData: ${record.date}`);
  };

  return (
    <tr>
      <td style={{ textAlign: "center" }}>{record.emoji}</td>
      <td>{record.title}</td>
      <td>{record.date}</td>
      <td>
        <button onClick={handleDetails}>Detalhes</button>
      </td>
    </tr>
  );
}

export default ListaHistorico;
