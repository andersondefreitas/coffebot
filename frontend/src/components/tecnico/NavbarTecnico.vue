<template>
  <header class="navbar-tecnico">
    <div class="nav-left">
      <router-link to="/home" class="logo-link">
        <img :src="logoIcon" alt="Logo Coffeebot" class="logo-icon" />
        <span>COFFEEBOT</span>
        <span class="area-badge">Área do Técnico</span>
      </router-link>
    </div>

    <div class="nav-right">
      <router-link to="/tecnico/dashboard" class="nav-link">
        <i class="material-icons">view_kanban</i>
        <span>Chamados</span>
      </router-link>
      
      <router-link to="/tecnico/graficos" class="nav-link">
        <i class="material-icons">bar_chart</i>
        <span>Dashboards</span>
      </router-link>
      
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
import { ref } from 'vue';
import { useRouter } from 'vue-router';

// --- CORREÇÃO AQUI ---
// Como este ficheiro está em 'components/tecnico/', precisamos
// subir 2 níveis ('../../') para encontrar a pasta 'assets'.
import logoIcon from '../../assets/logoCoffeebot.png';

const router = useRouter();
const isDropdownOpen = ref(false);

const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value;
};

const logout = () => {
  localStorage.removeItem('authToken');
  router.push('/');
  // Se você tiver problemas de estado, pode descomentar a linha abaixo
  // window.location.reload(); 
};
</script>

<style scoped>
/* Estilos 99% reaproveitados. 
   Apenas mudei o seletor principal e adicionei o .area-badge
*/
.navbar-tecnico {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 3rem;
  border-bottom: 1px solid #eee;
  background-color: #fff;
  font-family: 'Segoe UI', sans-serif;
  width: 100%;
  box-sizing: border-box;
}

.nav-left .logo-link {
  display: flex;
  align-items: center;
  text-decoration: none;
  color: #333;
  font-weight: bold;
  font-size: 1.5rem;
}

.logo-icon {
  height: 32px;
  margin-right: 0.5rem;
}

/* NOVO: Badge para a área do técnico */
.area-badge {
  font-size: 0.8rem;
  font-weight: 600;
  color: #f47b20; /* Cor de destaque do seu CTA */
  margin-left: 0.75rem;
  padding: 0.2rem 0.5rem;
  border: 1px solid #f47b20;
  border-radius: 12px;
}

.nav-right {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.nav-link {
  display: flex; /* Para alinhar ícone e texto */
  align-items: center;
  gap: 0.5rem; /* Espaço entre ícone e texto */
  text-decoration: none;
  color: #555;
  font-weight: 600;
  padding: 0.5rem;
  border-bottom: 2px solid transparent;
  transition: border-color 0.3s;
}

.nav-link:hover {
  border-color: #f47b20;
}

/* --- Estilos do Menu de Perfil (Idênticos) --- */
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
  top: 50px;
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
  color: #dc3545;
}
</style>