import { Link, useNavigate } from 'react-router-dom';
import './NavbarLogin.css'


const Navbar = () => {
  const navigate = useNavigate();
  const logout = () =>{
    localStorage.clear();
    navigate('/')
  }

  return (
    <nav className='container'>
      <h1>
        <Link to="/home">Emotions</Link>
      </h1>      
      <ul>
        <li onClick={() => navigate('/regemotion')}>Registrar Emoção</li>
        <li onClick={() => navigate('/historico')}>Historico</li>
        <li onClick={() => navigate('/relatorio')}>Relatório</li>
        <li onClick={() => navigate('/artigo')}>Biblioteca</li>
        <li onClick={() => logout()}>Logout</li>

      </ul>
    </nav>
  )
}

export default Navbar
