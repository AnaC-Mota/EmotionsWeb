import React, { useState, useEffect } from 'react';
import './Login.css';
import LOGO from '../../assets/LOGO.png';
import { signInWithEmailAndPassword, createUserWithEmailAndPassword, updateProfile, signInWithPopup } from 'firebase/auth';
import { auth, googleProvider} from '../../Firebase/app';
import { useNavigate } from 'react-router-dom';

const Login = ({ isSignUp, setIsSignUp }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  // login com email e senha
  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      // Se não tiver credencial
      if (isSignUp) {
        // Criar o usuário
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);
        const user = userCredential.user;
        await updateProfile(user, {
          displayName: name,
        });
        console.log('Usuário cadastrado:', user);
        navigate('/home');

      } else {
        // Realizar login
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        console.log('Usuário logado:', userCredential.user);

        const token = await userCredential.user.getIdToken();
        localStorage.setItem('authToken', token);

        navigate('/home');
      }
    } catch (error) {
      console.error('Erro:', error);
      setError(error.message);
    }
  };

  // login com Google
  const handleGoogleLogin = async () => {
    setError('');
    try {
      //autenticar o usuário com um popup
      const result = await signInWithPopup(auth, googleProvider);
      const user = result.user;
  
      console.log('Usuário autenticado pelo Google:', user);
  
      // Armazenar o token do usuário
      const token = await user.getIdToken();
      localStorage.setItem('authToken', token);
  
      navigate('/home');
    } catch (error) {
      console.error('Erro no login com Google:', error);
      setError('Erro ao autenticar com Google. Tente novamente.');
    }
  };

  //pra fazer o cadastro
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
            <p className="no-account-text">
              Não tem conta?{' '}
              <span className="create-account" onClick={handleSignUp}>
                Cadastre-se
              </span>
            </p>
          )}
        </div>
        {!isSignUp && (
          <div className="social-login-container">
            <button onClick={handleGoogleLogin} className="btn-google-circle">
              <img
                src="https://www.gstatic.com/firebasejs/ui/2.0.0/images/auth/google.svg"
                alt="Google Logo"
                className="google-logo"
              />
            </button>
          </div>
        )}
      </form>
    </div>
  );
};

export default Login;
