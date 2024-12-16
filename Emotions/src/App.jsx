// src/App.jsx
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Reg from './Pages/Reg';
import RegEmocion from './Pages/RegEmocion';
import Login from './Pages/Reg';
import Home from './Pages/Home';
import PrivateRouter from './components/PrivateRouter';
import Historico from './Pages/Histoico';

const App = () => {
    return (
    <Router>
      <div>
        <Routes>
          <Route path="/" element={<Login/>}/>
          <Route path="/home" element={<PrivateRouter><Home/></PrivateRouter>}/>
          <Route path="/historico" element={<PrivateRouter><Historico/></PrivateRouter>} />
          <Route path='/regemotion' element={<PrivateRouter><RegEmocion/></PrivateRouter>}/>
        </Routes>
      </div>
    </Router>
  );
};

export default App;
