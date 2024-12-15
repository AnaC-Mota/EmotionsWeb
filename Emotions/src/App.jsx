// src/App.jsx
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Navbar from './components/NavBar/navbar';
import Hero from './components/Hero/Hero';
import Programs from './components/Programs/Programs';
import Title from './components/Title/Title';
import Reg from './Pages/Reg';
import RegEmocion from './Pages/RegEmocion';

const App = () => {

  const Layout = ({ children }) => {
    const location = useLocation();
  
    
    const hideNavbar = location.pathname === '/login';
  };
    return (
    <Router>
      <div>
        <Routes>
          {/* Rota principal */}
          <Route path="/" element={
            <>
              <Navbar />
              <Hero />
              <div className="container">
                <Title />
                <Programs />
              </div>
            </>
          } />
          <Route path="/login" element={<Reg/>} />
          <Route path='/regemotion' element={<RegEmocion/>}/>
        </Routes>
      </div>
    </Router>
  );
};

export default App;
