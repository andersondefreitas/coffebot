<template>
  <div class="register-container">
    <div class="form-panel-register">
      <form @submit.prevent="executarCadastro" class="register-form">
        <h2>CRIAR CONTA</h2>
        <div class="input-group">
          <input type="text" placeholder="Nome Completo" v-model="nome" required />
        </div>
        <div class="input-group">
          <input type="email" placeholder="Email" v-model="email" required />
        </div>
        <div class="input-group">
          <input type="password" placeholder="Senha" v-model="senha" required />
        </div>
        
        <button type="submit" class="register-button" :disabled="isLoading">
          {{ isLoading ? 'CADASTRANDO...' : 'CADASTRAR' }}
        </button>

        <p v-if="mensagem" :class="sucesso ? 'success-message' : 'error-message'">{{ mensagem }}</p>

        <p class="login-link">
          Já tem uma conta? <router-link to="/">Faça o login</router-link>
        </p>
      </form>
    </div>
    <div class="brand-panel-register">
      <h1>BEM-VINDO!</h1>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const nome = ref('');
const email = ref('');
const senha = ref('');
const mensagem = ref('');
const sucesso = ref(false);
const isLoading = ref(false); // [MUDANÇA] Adicionado estado de loading

const router = useRouter();

const executarCadastro = async () => {
  mensagem.value = '';
  sucesso.value = false;
  isLoading.value = true; // [MUDANÇA] Ativa o loading

  try {
    // Chama o endpoint de registro do backend
    // !!! AJUSTE A PORTA se a sua API não estiver rodando na 5248 !!!
    await axios.post('http://localhost:5248/api/conta/registrar', {
      nome: nome.value,
      email: email.value,
      senha: senha.value
    });

    sucesso.value = true;
    mensagem.value = 'Cadastro realizado! Redirecionando para confirmação...';

    // --- [MUDANÇA PRINCIPAL] ---
    // Removemos o setTimeout e redirecionamos IMEDIATAMENTE
    // para a tela de confirmação, passando o email na URL.
    router.push(`/confirmar-email?email=${email.value}`);

  } catch (error) {
    sucesso.value = false;
    // Tenta pegar a mensagem de erro específica do backend (ex: "email já em uso")
    mensagem.value = error.response?.data?.mensagem || 'Ocorreu um erro ao tentar se cadastrar.';
  
  } finally {
    isLoading.value = false; // [MUDANÇA] Desativa o loading
  }
};
</script>

<style scoped>
/* Estilos similares aos de login para manter a consistência */
.register-container {
  display: flex;
  width: 100vw;
  height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
.form-panel-register {
  width: 50%;
  background-color: #ffffff;
  display: flex;
  justify-content: center;
  align-items: center;
}
.register-form {
  width: 350px;
}
.register-form h2 {
  font-size: 1.2rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
  letter-spacing: 1px;
  text-align: center;
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
.register-button {
  width: 100%;
  padding: 15px;
  border: none;
  border-radius: 4px;
  background-color: #f47b20;
  color: white;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
.register-button:hover {
  background-color: #d86713;
}

/* [MUDANÇA] Estilo para o botão desabilitado */
.register-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.brand-panel-register {
  width: 50%;
  background:  #d86713;
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 4rem;
  font-weight: 600;
}
.error-message {
  color: #d93025;
  margin-top: 1rem;
  text-align: center;
}
.success-message {
  color: #1e8e3e;
  margin-top: 1rem;
  text-align: center;
}
.login-link {
  text-align: center;
  margin-top: 1.5rem;
  color: #555;
}
.login-link a {
  color: #f47b20;
  font-weight: 600;
  text-decoration: none;
}
</style>