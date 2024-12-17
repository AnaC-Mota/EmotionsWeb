import React, { useState, useEffect } from 'react';
import './Login.css';
import LOGO from '../../assets/LOGO.png';
import { signInWithEmailAndPassword, createUserWithEmailAndPassword, updateProfile, signInWithPopup } from 'firebase/auth';
import { auth, googleProvider, facebookProvider } from '../../Firebase/app';
import { useNavigate } from 'react-router-dom';
import { FacebookAuthProvider } from 'firebase/auth';

const Login = ({ isSignUp, setIsSignUp }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate(); // Hook para navegação

  useEffect(() => {
    // Inicialização do Facebook SDK
    if (window.FB) {
      window.FB.init({
        appId: '549772114713245', // Substitua pelo seu App ID do Facebook
        cookie: true,
        xfbml: true,
        version: 'v10.0',
      });

      console.log("Facebook SDK carregado corretamente!");
    } else {
      console.error("Erro ao carregar o Facebook SDK.");
    }
  }, []);// A dependência vazia significa que a inicialização ocorrerá uma vez ao montar o componente

  // Função para login com email e senha
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

  // Função para login com Google
  const handleGoogleLogin = async () => {
    setError('');
    try {
      const result = await signInWithPopup(auth, googleProvider);
      const user = result.user;
  
      console.log('Usuário autenticado pelo Google:', user);
  
      // Armazenar o token do usuário (se necessário)
      const token = await user.getIdToken();
      localStorage.setItem('authToken', token);
  
      navigate('/home');
    } catch (error) {
      console.error('Erro no login com Google:', error);
      setError('Erro ao autenticar com Google. Tente novamente.');
    }
  };

  const handleFacebookLogin = () => {
    window.FB.login((response) => {
      if (response.authResponse) {
        const { accessToken } = response.authResponse;
        const credential = FacebookAuthProvider.credential(accessToken);

        signInWithPopup(auth, credential)
          .then((userCredential) => {
            const user = userCredential.user;
            console.log('Usuário autenticado pelo Facebook:', user);

            // Armazenando o token
            user.getIdToken().then((token) => {
              localStorage.setItem('authToken', token);
              navigate('/home');
            });
          })
          .catch((error) => {
            console.error('Erro ao autenticar com o Facebook:', error);
            setError('Erro ao autenticar com o Facebook. Tente novamente.');
          });
      } else {
        console.log('Falha no login do Facebook');
      }
    }, { scope: 'email' }); // Aqui você pode adicionar permissões que desejar (como 'email')
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
            <p className="no-account-text">
              Não tem conta?{' '}
              <span className="create-account" onClick={handleSignUp}>
                Cadastre-se
              </span>
            </p>
          )}
        </div>
        {/* Contêiner para os botões do Google e Facebook */}
        {!isSignUp && (
          <div className="social-login-container">
            <button onClick={handleGoogleLogin} className="btn-google-circle">
              <img
                src="https://www.gstatic.com/firebasejs/ui/2.0.0/images/auth/google.svg"
                alt="Google Logo"
                className="google-logo"
              />
            </button>
            <button onClick={handleFacebookLogin} className="btn-facebook-circle">
              <img
                src="https://upload.wikimedia.org/wikipedia/commons/5/51/Facebook_f_logo_%282019%29.svg"
                alt="Facebook Logo"
                className="facebook-logo"
              />
            </button>
          </div>
        )}
      </form>
    </div>
  );
};

export default Login;
