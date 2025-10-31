<template>
  <div class="page-container">
    
    <Navbar />

    <main class="hero-section">
      <div class="hero-text">
        <h1>PEÇA INTELIGENTE</h1>
        <h2 class="hero-emphasis">PEÇA COFFEEBOT</h2>
        <router-link to="/conhecer" class="know-more-link">
          Conhecer →
        </router-link>
      </div>
      <div class="hero-image">
        <img :src="posImage" alt="Terminal de Vendas Coffeebot" />
      </div>
    </main>
  </div>
</template>

<script setup>
// --- CORREÇÃO AQUI ---
// O 'Navbar.vue' está na mesma pasta (layout/) que este ficheiro,
// por isso o caminho é direto.
import Navbar from './Navbar.vue'; 
import posImage from '../../assets/terminais.png'; // <-- Também corrigi este caminho
</script>

<style scoped>
/* Estilo geral da página */
.page-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 100vw;
  background-color: #fff;
  font-family: 'Segoe UI', sans-serif;
  color: #333;
}


.hero-section {
  display: flex;
  align-items: flex-end;
  flex-grow: 1;
  padding: 0 0 0 4rem;
  gap: 2rem;
}

.hero-text {
  flex: 1;
  max-width: 500px;
  align-self: center;
}

.hero-text h1 {
  font-size: 3.5rem;
  font-weight: 300;
  margin: 0;
  line-height: 1.2;
}

.hero-text .hero-emphasis {
  font-size: 4.5rem;
  font-weight: 700;
  margin: 0;
  line-height: 1.2;
  white-space: nowrap;
}

.know-more-link {
  display: inline-block;
  margin-top: 2rem;
  font-size: 1.2rem;
  font-weight: 600;
  color: #333;
  text-decoration: none;
  transition: color 0.3s;
}

.know-more-link:hover {
  color: #f47b20;
}

.hero-image {
  flex: 1;
  display: flex;
  justify-content: flex-end;
  align-items: flex-end;
  overflow: hidden; 
}

.hero-image img {
  max-width: 150%;
  height: auto;
  max-height: 80vh; 
  margin-bottom: -10px;
}
</style>