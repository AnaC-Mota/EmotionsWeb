import React from 'react'
import { Link, useNavigate } from 'react-router-dom';
import './NavbarLogin.css'
import LOGO from '../../assets/LOGO.png'


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
        <li  onClick={() => navigate('/historico')}>Historico</li>
        <li>Relatório</li>
        <li>Biblioteca</li>
        <li  onClick={() => logout()}>Logout</li>

      </ul>
    </nav>
  )
}

export default Navbar
