<template>
  <div class="login-container">
    <div class="brand-panel">
      <div class="brand-content">
        <h1>COFFEEBOT</h1>
        <div class="logo">
          <img :src="logoIcon" alt="Coffeebot Logo Graphic" />
        </div>
      </div>
    </div>

    <div class="form-panel">
      <form @submit.prevent="executarLogin" class="login-form">
        <h2>LOG IN</h2>
        <div class="input-group">
          <input type="email" placeholder="Email" v-model="email" required />
        </div>
        <div class="input-group">
          <input type="password" placeholder="Senha" v-model="senha" required />
        </div>
        <button type="submit" class="login-button">ACESSAR</button>
        
        <p v-if="mensagemErro" class="error-message">{{ mensagemErro }}</p>

        <p class="register-link">
          Não tem uma conta? <router-link to="/register">Cadastre-se</router-link>
        </p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode'; // Importa o jwt-decode
import logoIcon from '../../assets/logoCoffeebot.png'; // Caminho corrigido

const email = ref('');
const senha = ref('');
const mensagemErro = ref('');

const router = useRouter();

const executarLogin = async () => {
  mensagemErro.value = '';
  try {
    
    const resposta = await axios.post('http://localhost:5248/api/conta/login', {
      email: email.value,
      senha: senha.value
    });
    
    const token = resposta.data.token; 
    localStorage.setItem('authToken', token);
    
    // Descodifica o token para ler o "cargo" (role)
    const decodedToken = jwtDecode(token);

    // --- CORREÇÃO CRÍTICA AQUI ---
    // O C# (ClaimTypes.Role) usa este nome longo como "chave" no token.
    const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']; 

    // Decide para onde redirecionar
    if (userRole === 'Tecnico' || userRole === 'Admin') {
      router.push('/tecnico/dashboard'); // <-- Redireciona Técnicos
    } else {
      router.push('/home'); // <-- Redireciona Clientes
    }

  } catch (error) {
    // Tenta usar a mensagem de erro específica do backend
    if (error.response && error.response.data && error.response.data.mensagem) {
      mensagemErro.value = error.response.data.mensagem;
    } else {
      mensagemErro.value = error.response?.data?.Mensagem || 'Email ou senha inválidos. Tente novamente.';
    }
    localStorage.removeItem('authToken');
  }
};
</script>

<style scoped>
.login-container {
  display: flex;
  width: 100vw;
  height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
.brand-panel {
  width: 50%;
  background: #f47b20;
  color: white;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 40px;
  text-align: center;
}
.brand-content h1 {
  font-size: 3.5rem;
  margin-bottom: 2rem;
  font-weight: 600;
  letter-spacing: 2px;
}
.logo img {
  width: 150px;
  height: auto;
  opacity: 1;
}
.form-panel {
  width: 50%;
  background-color: #ffffff;
  display: flex;
  justify-content: center;
  align-items: center;
}
.login-form {
  width: 350px;
}
.login-form h2 {
  font-size: 1.2rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
  letter-spacing: 1px;
}
.input-group {
  margin-bottom: 1.5rem;
}
.input-group input {
  width: 100%;
  padding: 15px;
  border: 1px solid #d9d9d9;
  border-radius: 4px;
  font-size: 1rem;
}
.login-button {
  width: 100%;
  padding: 15px;
  border: none;
  border-radius: 4px;
  background-color: #f47b20;
  color: white;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color .3s ease;
}
.login-button:hover {
  background-color: #d86713;
}
.error-message {
  color: #d93025;
  margin-top: 1rem;
  text-align: center;
}
.register-link {
  text-align: center;
  margin-top: 1.5rem;
  color: #555;
}
.register-link a {
  color: #f47b20;
  font-weight: 600;
  text-decoration: none;
}
.register-link a:hover {
  text-decoration: underline;
}
</style>