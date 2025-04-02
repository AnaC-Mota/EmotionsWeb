import  { useState } from 'react';
import Login from '../components/Login/Login';

const Reg = () => {
  const [isSignUp, setIsSignUp] = useState(false); 

  return (
    <div className="reg-container">
      <Login isSignUp={isSignUp} setIsSignUp={setIsSignUp} />
    </div>
  );
};

export default Reg;