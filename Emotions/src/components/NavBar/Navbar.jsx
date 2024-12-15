import React from 'react'
import { useNavigate } from 'react-router-dom';
import './Navbar.css'
import LOGO from '../../assets/LOGO.png'


const Navbar = () => {
  const navigate = useNavigate();

  return (
    <nav className='container'>
      <img src={LOGO} alt="" className='logo' />
      
      <ul>
        <li>HOME</li>
        <li>ABOUT US</li>
        <li><button onClick={() => navigate('/regemotion')}>Registrar Emoção</button></li>
        <li><button className='btn'onClick={() => navigate('/login')}>LOG IN</button></li>
        <li><button className='btn' onClick={() => navigate('/login')}>SIG UP</button></li>
      </ul>
    </nav>
  )
}

export default Navbar
