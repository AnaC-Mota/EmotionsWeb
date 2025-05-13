import { useEffect, useState } from "react";
import EmotionService from "../../services/emotion-services";
import './Artigo.css';

const App = () => {
  const [news, setNews] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    EmotionService.getNews()
      .then((response) => {
        setNews(response.data);
      })
      .catch((error) => {
        console.error("Erro ao buscar notícias:", error);
      })
      .finally(() => setLoading(false));
  }, []);

  return (
   <div className="container">
  <h1>Notícias sobre Saúde Mental</h1>
  {loading ? (
    <p>Carregando notícias...</p>
  ) : news.length === 0 ? (
    <p>Nenhuma notícia disponível no momento.</p>
  ) : (
    <div className="news-cards">
      {news.map((article, index) => (
        <a key={index} href={article.url} target="_blank" rel="noopener noreferrer" className="news-card-link">
          <div className="news-card">
            <img
              src={article.urlToImage || "/placeholder.png"}
              alt={article.title}
              className="news-image"
            />
            <h3 className="news-title">{article.title}</h3>
            <p className="news-description">{article.description}</p>
          </div>
        </a>
      ))}
    </div>
  )}
</div>
  );
};

export default App;
