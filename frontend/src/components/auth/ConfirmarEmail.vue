<template>
  <div class="confirmation-container">
    <h1>Confirme sua Conta</h1>
    <p class="subtitle">
      Enviamos um código de 6 dígitos para o seu e-mail. Por favor,
      insira-o abaixo para ativar sua conta.
    </p>

    <form @submit.prevent="handleConfirmacao" class="confirmation-form">
      <div class="form-group">
        <label for="email">E-mail</label>
        <input
          type="email"
          id="email"
          v-model="email"
          placeholder="seu.email@exemplo.com"
          required
        />
      </div>

      <div class="form-group">
        <label for="codigo">Código de 6 dígitos</label>
        <input
          type="text"
          id="codigo"
          v-model="codigo"
          placeholder="123456"
          maxlength="6"
          required
        />
      </div>

      <p v-if="mensagem" :class="isError ? 'error-message' : 'success-message'">
        {{ mensagem }}
      </p>

      <button type="submit" class="confirm-button" :disabled="isLoading">
        {{ isLoading ? 'Verificando...' : 'Confirmar Conta' }}
      </button>
    </form>

    <router-link v-if="processoFinalizado" to="/" class="login-link">
      Ir para a página de Login
    </router-link>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

// Variáveis reativas do formulário
const email = ref('');
const codigo = ref('');

// Variáveis de estado
const isLoading = ref(false);
const mensagem = ref('');
const isError = ref(false);
const processoFinalizado = ref(false);

const route = useRoute(); // Para pegar o email da URL, se passarmos
const router = useRouter(); // Para redirecionar para o login

// Tenta pegar o email da URL (se for passado do cadastro)
// Ex: /confirmar-email?email=teste@gmail.com
onMounted(() => {
  if (route.query.email) {
    email.value = route.query.email;
  }
});

const handleConfirmacao = async () => {
  isLoading.value = true;
  mensagem.value = '';
  isError.value = false;
  processoFinalizado.value = false;

  try {
    // 1. Monta o DTO que o backend espera
    const payload = {
      email: email.value,
      codigo: codigo.value,
    };

    // 2. Envia para o NOVO endpoint [HttpPost]
    // !!! AJUSTE A PORTA se a sua API não estiver rodando na 5248 !!!
    await axios.post(
      'http://localhost:5248/api/conta/confirmar-email',
      payload
    );

    // 3. Sucesso
    mensagem.value = 'E-mail confirmado com sucesso! Você já pode fazer o login.';
    isError.value = false;
    processoFinalizado.value = true; // Mostra o link para o login

  } catch (error) {
    // 4. Erro
    mensagem.value = 'Ocorreu um erro: o e-mail está incorreto ou o código é inválido/expirado.';
    isError.value = true;
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.confirmation-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  text-align: center;
  font-family: 'Segoe UI', sans-serif;
  padding: 2rem;
  background-color: #f9f9f9;
}

h1 {
  color: #333;
}

.subtitle {
  font-size: 1.1rem;
  color: #555;
  max-width: 400px;
  margin-bottom: 2rem;
}

.confirmation-form {
  width: 100%;
  max-width: 400px;
  background: #fff;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.form-group {
  margin-bottom: 1.5rem;
  text-align: left;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #444;
}

.form-group input {
  width: 100%;
  padding: 12px;
  font-size: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  box-sizing: border-box; /* Garante que o padding não aumente a largura */
}

.confirm-button {
  width: 100%;
  padding: 12px;
  background-color: #f47b20;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  font-weight: bold;
  font-size: 1rem;
  border: none;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.confirm-button:hover {
  background-color: #d86713;
}

.confirm-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.error-message {
  color: #d9534f;
  margin-bottom: 1rem;
}

.success-message {
  color: #5cb85c;
  margin-bottom: 1rem;
}

.login-link {
  margin-top: 1.5rem;
  padding: 10px 20px;
  color: #f47b20;
  text-decoration: none;
  font-weight: bold;
  transition: color 0.3s ease;
}

.login-link:hover {
  color: #d86713;
}
</style>