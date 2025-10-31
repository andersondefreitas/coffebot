// Arquivo: src/router/index.js 
import { createRouter, createWebHistory } from 'vue-router'

// --- 1. GRUPO: AUTENTICAÇÃO ---
import Login from '../components/auth/login.vue'
import Register from '../components/auth/Register.vue' 
import ConfirmarEmail from '../components/auth/ConfirmarEmail.vue'
import RedefinirSenha from '../components/auth/RedefinirSenha.vue'

// --- 2. GRUPO: LAYOUT E PÁGINAS GERAIS ---
import TelaInicial from '../components/layout/TelaInicial.vue'
import Conhecer from '../components/geral/Conhecer.vue'

// --- 3. GRUPO: LOJA ---
import NossosProdutos from '../components/loja/NossosProdutos.vue';
import Carrinho from '../components/loja/Carrinho.vue';

// --- 4. GRUPO: CHAMADOS (Cliente) ---
import AbrirChamado from '../components/chamados/abrirchamado.vue'
import Chatbot from '../components/chamados/chatbot.vue'

// --- 5. GRUPO: TÉCNICO ---
import TecnicoDashboard from '../components/tecnico/TecnicoKanban.vue';
import GraficosDashboard from '../components/tecnico/GraficosPlaceholder.vue'; // (Assumindo que criou este ficheiro)


const routes = [
  // --- ROTAS PÚBLICAS (não precisam de login) ---
  { path: '/', name: 'Login', component: Login },
  { path: '/register', name: 'Register', component: Register },
  { path: '/confirmar-email', name: 'ConfirmarEmail', component: ConfirmarEmail },
  { path: '/redefinir-senha', name: 'RedefinirSenha', component: RedefinirSenha },
  { path: '/produtos', name: 'NossosProdutos', component: NossosProdutos },
  { path: '/conhecer', name: 'Conhecer', component: Conhecer },

  // --- ROTAS PROTEGIDAS (exigem login) ---
  { 
    path: '/home', 
    name: 'Home', 
    component: TelaInicial, 
    meta: { requiresAuth: true }
  },
  { 
    path: '/suporte', 
    name: 'Suporte', 
    component: AbrirChamado, 
    meta: { requiresAuth: true } 
  },
  { 
    path: '/chatbot-ia', 
    name: 'ChatbotIA', 
    component: Chatbot, 
    meta: { requiresAuth: true } 
  },
  { 
    path: '/carrinho', 
    name: 'Carrinho', 
    component: Carrinho, 
    meta: { requiresAuth: true } 
  }, 
  { 
    path: '/tecnico/dashboard', 
    name: 'TecnicoDashboard', 
    component: TecnicoDashboard, 
    meta: { requiresAuth: true } 
  },
  
  // --- CORREÇÃO AQUI ---
  // A rota foi movida para DENTRO do array 'routes'
  { 
    path: '/tecnico/graficos', 
    name: 'GraficosDashboard', 
    component: GraficosDashboard, 
    meta: { requiresAuth: true } 
  }
  // --------------------

] // <-- O array fecha aqui

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes
})

// (O seu 'beforeEach' está correto e foi mantido)
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const token = localStorage.getItem('authToken');
  if (requiresAuth && !token) {
    next({ name: 'Login' });
  } else {
    next();
  }
});

export default router