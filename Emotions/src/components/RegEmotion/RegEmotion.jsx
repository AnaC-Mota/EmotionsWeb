import React, { useState } from 'react';
import axios from 'axios';
import './RegEmotion.css';

const emojiList = [
  '😀', '😃', '😄', '😁', '😆', '😅', '😂', '🤣', '😊', '😇', 
  '🙂', '🙃', '😉', '😌', '😍', '😘', '😗', '😙', '😚', 
  '😋', '😜', '😝', '😛', '🤑', '🤗', '🤩', '🤔', '😐',
  '😑', '😶', '😏', '😒', '😞', '😔', '😟', '😕', '🙁',
  '☹️', '😣', '😖', '😫', '😩', '😢', '😭', '😤', '😠',
  '😡', '🤬', '😈', '👿', '👹', '👺', '🤡', '💩', '👻', 
  '👽', '💀', '☠️'
];

const RegEmotion = () => {
  const [title, setTitle] = useState('');
  const [emotions, setEmotions] = useState('');
  const [description, setDescription] = useState('');
  const [selectedEmoji, setSelectedEmoji] = useState(null);
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const [registeredEmotions, setRegisteredEmotions] = useState([]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Divide as emoções separadas por vírgula em um array
    const emotionArray = emotions.split(',').map(emotion => emotion.trim());

    // Armazena o novo título, array de emoções e descrição no estado
    const newEmotion = { 
      titulo: title, 
      emocao: emotionArray.join(', '), // Junção das emoções em uma string
      descricao: description,
      emoji: selectedEmoji
    };

    
    try {
      const response = await fetch('http://localhost:5000/api/registrosEmocoes', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(novoRegistro),
      });

      if (response.ok) {
        alert('Registro de emoção salvo com sucesso!');
      } else {
        alert('Erro ao salvar o registro.');
      }
    } catch (error) {
      console.error('Erro:', error);
      alert('Erro ao se comunicar com o servidor.');
    }
  };

  return (
    <div className="regemotion-container">
      <form onSubmit={handleSubmit}>
        <label className='text-label'>
          Título:
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Digite um título"
            required
          />
        </label>
        
        <div className="emoji-picker-container">
            <div
            className="emoji-circle"
            onClick={() => setShowEmojiPicker(!showEmojiPicker)}
            >
            +
            </div>
            {showEmojiPicker && (
            <div className="emoji-list">
                {emojiList.map((emoji, index) => (
                <span 
                    key={index} 
                    onClick={() => {
                    setSelectedEmoji(emoji);
                    setShowEmojiPicker(false); // Fecha a lista após a seleção
                    }}
                    className="emoji-item"
                >
                    {emoji}
                </span>
                ))}
            </div>
            )}
        </div>
        <div>
        <label className="emotion-label">
            Emoções (separadas por vírgula):
            <input
            type="text"
            value={emotions}
            onChange={(e) => setEmotions(e.target.value)}
            placeholder="Digite suas emoções, separadas por vírgula"
            required
            />
        </label>
        </div>

        <label>
          Descrição:
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Descreva seu dia"
            required
          ></textarea>
        </label>
        <button type="submit" className="submit-button">Registrar</button>
      </form>

    </div>
  );
};

export default RegEmotion;
