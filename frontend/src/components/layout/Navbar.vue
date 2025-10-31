<template>
  <header class="navbar">
    <div class="nav-left">
      <router-link to="/home" class="logo-link">
        <img :src="logoIcon" alt="Logo Coffeebot" class="logo-icon" />
        <span>COFFEEBOT</span>
      </router-link>
    </div>

    <div v-if="!isLoggedIn" class="nav-right">
      <router-link to="/produtos" class="nav-link">Nossos Produtos</router-link>
      <router-link to="/register" class="nav-link">Abra sua conta</router-link>
      <router-link to="/" class="nav-button-cta">Acesse sua conta</router-link>
    </div>

    <div v-else class="nav-right">
      <router-link to="/produtos" class="nav-link">Nossos Produtos</router-link>
      <router-link to="/suporte" class="nav-link">Suporte Técnico</router-link>
      
      <div class="profile-menu">
        <button @click="toggleDropdown" class="profile-button">
          <i class="material-icons">person</i>
        </button>

        <div v-if="isDropdownOpen" class="dropdown-menu">
          <router-link to="/alterar-senha" class="dropdown-item">Alterar Senha</router-link>
          <button @click="logout" class="dropdown-item logout-button">Sair</button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

// --- CORREÇÃO AQUI ---
// Como este ficheiro está em 'components/layout/', precisamos
// subir 2 níveis ('../../') para encontrar a pasta 'assets'.
import logoIcon from '../../assets/logoCoffeebot.png';

const router = useRouter();

// Estado para controlar a visibilidade do dropdown
const isDropdownOpen = ref(false);

// Propriedade computada para verificar se o usuário está logado
// Ele verifica se o token existe no localStorage.
const isLoggedIn = computed(() => {
  return !!localStorage.getItem('authToken');
});

const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value;
};

// Função de Logout
const logout = () => {
  // 1. Remove o token do navegador
  localStorage.removeItem('authToken');
  
  // 2. Redireciona para a página de login
  router.push('/');

  // Opcional: Força o recarregamento para limpar qualquer estado residual
  // window.location.reload(); 
};
</script>

<style scoped>
/* Seus estilos existentes da navbar... */
.navbar { display: flex; justify-content: space-between; align-items: center; padding: 1rem 3rem; border-bottom: 1px solid #eee; background-color: #fff; font-family: 'Segoe UI', sans-serif; width: 100%; box-sizing: border-box; }
.nav-left .logo-link { display: flex; align-items: center; text-decoration: none; color: #333; font-weight: bold; font-size: 1.5rem; }
.logo-icon { height: 32px; margin-right: 0.5rem; }
.nav-right { display: flex; align-items: center; gap: 1.5rem; }
.nav-link { text-decoration: none; color: #555; font-weight: 600; padding: 0.5rem; border-bottom: 2px solid transparent; transition: border-color 0.3s; }
.nav-link:hover { border-color: #f47b20; }
.nav-button-cta { background-color: #f47b20; color: white; padding: 0.75rem 1.5rem; border-radius: 25px; font-weight: bold; text-decoration: none; cursor: pointer; transition: background-color 0.3s; }
.nav-button-cta:hover { background-color: #d86713; }

/* --- NOVOS ESTILOS PARA O MENU DE PERFIL --- */
.profile-menu {
  position: relative;
}

.profile-button {
  background-color: #f0f0f0;
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.profile-button .material-icons {
  color: #555;
}

.dropdown-menu {
  position: absolute;
  top: 50px; /* Posição abaixo do ícone */
  right: 0;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.1);
  width: 180px;
  overflow: hidden;
  z-index: 100;
}

.dropdown-item {
  display: block;
  width: 100%;
  padding: 12px 16px;
  text-align: left;
  text-decoration: none;
  color: #333;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1rem;
}

.dropdown-item:hover {
  background-color: #f8f9fa;
}

.logout-button {
  color: #dc3545; /* Cor vermelha para a opção de sair */
}
</style>