// src/components/Login/Login.jsx
import React, { useState } from 'react';
import './Login.css';
import LOGO from '../../assets/LOGO.png';

const Login = ({ isSignUp, setIsSignUp }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');

  const handleLoginSubmit = (e) => {
    e.preventDefault();
    // Lógica de login aqui
    console.log({ email, password, name });
  };

  const handleSignUp = () => {
    setIsSignUp(true);
  };

  const handleForgotPassword = () => {
    // Lógica para recuperação de senha
    alert('Redirecionar para a página de recuperação de senha.');
  };

  return (
    <div className="login-container">
      <img src={LOGO} alt="Site Logo" className="logo" />

      <form onSubmit={handleLoginSubmit} className="login-form">
        {/* Título dinâmico */}
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


