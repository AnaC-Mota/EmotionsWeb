// src/App.jsx
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import RegEmocion from './Pages/RegEmocion';
import Login from './Pages/Reg';
import Home from './Pages/Home';
import PrivateRouter from './components/PrivateRouter';
import Historico from './Pages/Histoico';
import Relatorio from './Pages/Relatorio';
import GerarRelatorio from './components/Relatorio/Gerar';
import Artigo from './Pages/Artigo';

const App = () => {
    return (
    <Router>
      <div>
        <Routes>
          <Route path="/" element={<Login/>}/>
          <Route path="/home" element={<PrivateRouter><Home/></PrivateRouter>}/>
          <Route path="/historico" element={<PrivateRouter><Historico/></PrivateRouter>} />
          <Route path='/regemotion' element={<PrivateRouter><RegEmocion/></PrivateRouter>}/>
          <Route path='/relatorio' element={<PrivateRouter><Relatorio/></PrivateRouter>}/>
          <Route path="/gerar-relatorio" element={<PrivateRouter><GerarRelatorio/></PrivateRouter>} />
          <Route path='/artigo' element={<PrivateRouter><Artigo/></PrivateRouter>}/>

        </Routes>
      </div>
    </Router>
  );
};

export default App;
