// src/Firebase/app.js
import { initializeApp } from "firebase/app";
import { getAuth, GoogleAuthProvider, FacebookAuthProvider } from "firebase/auth";

// Configuração do Firebase
const firebaseConfig = {
  apiKey: "AIzaSyBCK_Q-pU-UIZfyXH0iDYQ4V_pH-BxtrrY",
  authDomain: "emocoes-4f9b5.firebaseapp.com",
  projectId: "emocoes-4f9b5",
  storageBucket: "emocoes-4f9b5.firebasestorage.app",
  messagingSenderId: "217645884982",
  appId: "1:217645884982:web:def5ebb400c600f3061a97",
  measurementId: "G-PZBG8WV7P3"
};

// Inicializar Firebase
const app = initializeApp(firebaseConfig);

// Inicializar o serviço de autenticação
const auth = getAuth(app);
const googleProvider = new GoogleAuthProvider();
const facebookProvider = new FacebookAuthProvider();


export { auth, googleProvider, facebookProvider  };
