import React, { useState } from 'react';
import './Login.css';
import LOGO from '../../assets/LOGO.png';
import { signInWithEmailAndPassword, createUserWithEmailAndPassword, updateProfile } from 'firebase/auth';
import { auth } from '../../Firebase/app'; 
import { useNavigate } from 'react-router-dom'; 

const Login = ({ isSignUp, setIsSignUp }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate(); // Hook para navegação

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      if (isSignUp) {
        // Criar o usuário com e-mail e senha
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);
        const user = userCredential.user;
  
        // Adicionar o nome do usuário
        await updateProfile(user, {
          displayName: name,
        });
  
        console.log('Usuário cadastrado:', user);
        alert('Conta criada com sucesso!');
  
        // Redirecionar para a Home
        navigate('/home');
      } else {
        // Realizar login
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        console.log('Usuário logado:', userCredential.user);
  
        const token = await userCredential.user.getIdToken();
        localStorage.setItem('authToken', token);
  
        alert('Login bem-sucedido!');
        navigate('/home');
      }
    } catch (error) {
      console.error('Erro:', error);
      setError(error.message);
    }
  };
  
  const handleSignUp = () => {
    setIsSignUp(true);
  };

  const handleForgotPassword = () => {
    alert('Redirecionar para a página de recuperação de senha.');
  };

  return (
    <div className="login-container">
      <img src={LOGO} alt="Site Logo" className="logo" />

      <form onSubmit={handleLoginSubmit} className="login-form">
        <h2>{isSignUp ? 'Sign Up' : 'Login'}</h2>

        {isSignUp && (
          <input
            type="text"
            placeholder="Nome"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
        )}

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Senha"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        {error && <p className="error-message">{error}</p>}

        <div className="forgot-password" onClick={handleForgotPassword}>
          Esqueceu a senha?
        </div>

        <div className="button-container">
          <button type="submit" className="btn-login">
            {isSignUp ? 'Cadastrar' : 'Login'}
          </button>

          {!isSignUp && (
            <button type="button" className="btn-signup" onClick={handleSignUp}>
              Criar Conta
            </button>
          )}
        </div>
      </form>
    </div>
  );
};

export default Login;