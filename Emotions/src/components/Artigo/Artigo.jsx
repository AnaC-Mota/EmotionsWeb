import { useEffect, useState } from "react";
import EmotionService from "../../services/emotion-services"; // Certifique-se de que o caminho está correto

const App = () => {
  const [news, setNews] = useState([]);
  const [loading, setLoading] = useState(true); // Estado para controlar o carregamento

  useEffect(() => {
    EmotionService.getNews()
      .then((response) => {
        console.log("Dados recebidos:", response.data);
        setNews(response.data);
      })
      .catch((error) => {
        console.error("Erro ao buscar notícias:", error);
      })
      .finally(() => setLoading(false)); // Define "loading" como falso após a resposta
  }, []);

  return (
    <div className="container">
      <h1>Notícias sobre Saúde Mental</h1>
      {loading ? (
        <p>Carregando notícias...</p>
      ) : news.length === 0 ? (
        <p>Nenhuma notícia disponível no momento.</p>
      ) : (
        news.map((article, index) => (
          <div key={index} className="news-card">
            <h2>{article.title}</h2>
            <p>{article.description}</p>
            <a href={article.url} target="_blank" rel="noopener noreferrer">
              Leia mais
            </a>
          </div>
        ))
      )}
    </div>
  );
};

export default App;
