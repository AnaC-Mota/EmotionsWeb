import React from 'react'
import { useNavigate } from 'react-router-dom';
import './NavbarLogin.css'
import LOGO from '../../assets/LOGO.png'


const Navbar = () => {
  const navigate = useNavigate();

  return (
    <nav className='container'>
      <img src={LOGO} alt="" className='logo' />
      
      <ul>
        <li><button onClick={() => navigate('/regemotion')}>Registrar Emoção</button></li>
        <li>Histórico</li>
        <li>Relatório</li>
        <li>Biblioteca</li>
      </ul>
    </nav>
  )
}

export default Navbar
