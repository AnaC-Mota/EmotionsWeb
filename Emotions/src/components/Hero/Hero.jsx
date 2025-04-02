import './Hero.css'
import { useNavigate } from 'react-router-dom';


const Hero = () => {
    const navigate = useNavigate();
  
  return (
    <div className='hero'>
    <div className='hero-text'>
          <h1>RELATE E RELAXE</h1>
          <p>TORNE SUA VIDA MAIS LEVE E FELIZ.</p>
          <button className='btn' onClick={() => navigate('/regemotion')}>Escreva seu primeiro sentimento</button>
    </div>
    </div>
        
  )
}

export default Hero
