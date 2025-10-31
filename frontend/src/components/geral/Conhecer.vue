<template>
  <div class="conhecer-page-wrapper">
    <Navbar />

    <section class="about-section">
      <div class="container">
        <div class="text-content">
          <h2>O QUE FAZEMOS?</h2>
          <p>
            Trabalhamos com sistemas de automação de caixa que facilita e agiliza o trabalho dos atendentes. O cardápio via totem se conecta automaticamente à nossa maquininha, oferecendo uma solução personalizada para o seu negócio. Aceitando Pix, cartões de débito, crédito e até vouchers, tudo de forma simples e integrada.
          </p>
          <router-link to="/register" class="cta-button-dark">abra sua conta</router-link>
        </div>
        <div class="image-content">
          <img :src="mainImage" alt="Atendente de padaria feliz" class="main-image" />
          
          <div class="overlay-card pix-card">
            <i class="material-icons">check_circle</i>
            <span>Pix recebido</span>
            <strong>35,50</strong>
          </div>
        </div>
      </div>
    </section>

    <section class="features-section">
      <div class="container">
        <h2>Motivos pra virar <strong>Coffeebot</strong></h2>
        <div class="features-grid">
          <div v-for="feature in features" :key="feature.id" class="feature-card">
            <div class="feature-image-wrapper">
              <img :src="feature.image" :alt="feature.title" />
            </div>
            <h3>{{ feature.title }}</h3>
            <p>{{ feature.description }}</p>
          </div>
        </div>
        <div class="cta-container">
          <router-link to="/produtos" class="cta-button-orange">Conferir nossos produtos</router-link>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
// --- CORREÇÃO AQUI ---
// O Navbar está na pasta 'layout', não 'geral'
import Navbar from '../layout/Navbar.vue';

// Imports das imagens e da lógica
import { ref } from 'vue';

// --- CORREÇÃO AQUI ---
// Como este ficheiro está em 'components/geral/', precisamos
// subir 2 níveis ('../../') para encontrar a pasta 'assets'.
import mainImage from '../../assets/sobre.png'; 
import agilidadeImg from '../../assets/agilidade.png';
import pagamentosImg from '../../assets/pagamentos-conectados.png';
import personalizavelImg from '../../assets/personalizavel.png';
import interfaceImg from '../../assets/interface-intuitiva.png';
import inclusaoImg from '../../assets/inclusao.png';
import suporteImg from '../../assets/suporte-tecnico.png';

const features = ref([
  { id: 1, image: agilidadeImg, title: 'agilidade + praticidade', description: 'Simplifique o dia a dia dos empreendedores com soluções que reduzem burocracias e tornam os pagamentos mais rápidos, fáceis e seguros.' },
  { id: 2, image: pagamentosImg, title: 'pagamentos conectados', description: 'Integração simples e eficiente que conecta diferentes formas de pagamento em um só lugar. Mais controle, menos complicação.' },
  { id: 3, image: personalizavelImg, title: 'personalizavel', description: 'Adapte as soluções ao jeito do seu negócio. Layouts, recursos e configurações flexíveis que se moldam às suas necessidades.' },
  { id: 4, image: interfaceImg, title: 'interface intuitiva', description: 'Design simples e funcional, pensado para que qualquer pessoa use sem complicação. Menos cliques, mais eficiência.' },
  { id: 5, image: inclusaoImg, title: 'inclusão', description: 'Uma ferramenta que facilita pedidos de forma simples e acessível. Ideal para clientes e funcionários mais tímidos.' },
  { id: 6, image: suporteImg, title: 'suporte técnico', description: 'Atendimento rápido e eficiente para resolver qualquer dúvida ou problema. Uma equipe preparada para dar todo o apoio necessário.' }
]);
</script>

<style scoped>
.conhecer-page-wrapper {
  overflow-x: hidden; /* Esconde qualquer conteúdo que vaze para os lados */
}
/* Estilos da Seção 1 (O Que Fazemos?) */
.about-section { padding: 6rem 2rem; background-color: #fff; font-family: 'Segoe UI', sans-serif; }
.container { max-width: 1100px; margin: 0 auto; display: flex; align-items: center; gap: 4rem; flex-wrap: wrap; }
.text-content { flex: 1; min-width: 300px; }
.text-content h2 { font-size: 3rem; font-weight: 700; color: #333; margin-top: 0; margin-bottom: 1.5rem; }
.text-content p { font-size: 1.1rem; color: #6c757d; line-height: 1.7; margin-bottom: 2rem; }
.cta-button-dark { background-color: #212529; color: white; padding: 1rem 2.5rem; border-radius: 25px; text-decoration: none; font-weight: bold; transition: background-color 0.3s, transform 0.2s; display: inline-block; }
.cta-button-dark:hover { background-color: #343a40; transform: translateY(-2px); }
.image-content { flex: 1; position: relative; display: flex; align-items: center; justify-content: center; min-width: 300px; }
.main-image { width: 100%; border-radius: 12px; box-shadow: 0 10px 25px rgba(0,0,0,0.1); }
.overlay-card { position: absolute; background: white; border-radius: 12px; box-shadow: 0 5px 15px rgba(0,0,0,0.15); display: flex; align-items: center; padding: 0.75rem; }

/* Estilos do .qr-card removidos */

.pix-card { bottom: -1.5rem; right: 2rem; gap: 0.5rem; }
.pix-card .material-icons { color: #28a745; font-size: 1.5rem; }
.pix-card span { font-size: 0.9rem; color: #6c757d; }
.pix-card strong { font-size: 1rem; color: #333; }

/* Estilos da Seção 2 (Motivos) */
.features-section { padding: 6rem 2rem; background-color: #f8f9fa; font-family: 'Segoe UI', sans-serif; }
.features-section .container { flex-direction: column; }
.features-section h2 { font-size: 2.5rem; text-align: center; margin-bottom: 4rem; color: #333; }
.features-section h2 strong { font-weight: 700; }
.features-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 2.5rem; width: 100%; }
.feature-card { text-align: left; }
.feature-image-wrapper { background-color: #fff; border: 1px solid #e9ecef; border-radius: 12px; padding: 1.5rem; display: inline-block; margin-bottom: 1.5rem; }
.feature-image-wrapper img { display: block; height: 120px; width: auto; }
.feature-card h3 { font-size: 1.2rem; font-weight: 600; text-transform: uppercase; color: #333; margin-bottom: 0.5rem; }
.feature-card p { color: #6c757d; line-height: 1.6; }
.cta-container { text-align: center; margin-top: 4rem; }
.cta-button-orange { background-color: #f47b20; color: white; padding: 1rem 3rem; border-radius: 30px; font-size: 1.1rem; font-weight: bold; text-decoration: none; transition: background-color 0.3s, transform 0.2s; display: inline-block; }
.cta-button-orange:hover { background-color: #d86713; transform: translateY(-2px); }
</style>