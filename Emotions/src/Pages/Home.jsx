// src/Pages/Home.jsx
import React from 'react';
import NavbarLogin from '../components/NavBarLogin/NavBarLogin';
import Hero from '../components/Hero/Hero';
import Title from '../components/Title/Title';
import Programs from '../components/Programs/Programs';

const Home = () => {
  return (
    <div>
      <NavbarLogin/>
      <Hero />
      <Title />
      <Programs />
    </div>
  );
};

export default Home;
