import { useEffect, useState } from 'react';
import './RegEmotion.css';
import {APIService}  from "../../http-common";
import { useNavigate } from 'react-router-dom';

//lista de emojis
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
  const [title, setTitle] = useState("");
  const [emotions, setEmotions] = useState('');
  const [description, setDescription] = useState('');
  const [selectedEmoji, setSelectedEmoji] = useState(null);
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Armazena os campos de reistro
    const newEmotion = { 
      titulo: title, 
      emocao: emotions,
      descricao: description,
      emoji: selectedEmoji,
    };

    
    try {
      const response = await APIService.Axios().post("Home/AddDocument", newEmotion);
      console.log(response.data)
      if (response.status==200) {
        navigate('/historico');

      } else {
        alert('Erro ao salvar o registro.');
      }
    } catch (error) {
      console.error('Erro:', error);
      alert('Erro ao se comunicar com o servidor.');
    }
  };

  const fetchData  = async () => {
    try {
      const response = await APIService.Axios().get("Home/GetAllDocuments");
      console.log(response.data)
    } catch (error) {
      console.error('Erro: ', error);
    }
  };

  useEffect(()=>{
    fetchData()
  }, [])

  return (
    <div className="regemotion-container">
      <form onSubmit={handleSubmit} className='regemotion-form'>
         <div className='regemotion-formleft'>
            <div className="emoji-picker-container">
              <div
                className="emoji-circle"
                onClick={() => setShowEmojiPicker(!showEmojiPicker)}
                >
                {
                  selectedEmoji != null ? (
                    <span>{selectedEmoji}</span>
                  ) : (<div>+</div>)
                }
              </div>
              {showEmojiPicker && (
                <div className="emoji-list">
                    {emojiList.map((emoji, index) => (
                    <span 
                        key={index} 
                        onClick={() => {
                        setSelectedEmoji(emoji);
                        console.log(emoji)
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
          </div>
        
        <div className='regemotion-formright'>
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
        </div>
      </form>

    </div>
  );
};

export default RegEmotion;