<template>
  <div class="reset-container">
    <div class="reset-form">
      <h2>Redefinir Senha</h2>
      <p v-if="!tokenValido">Token inválido ou não encontrado. Por favor, solicite um novo link de redefinição.</p>
      
      <form v-else @submit.prevent="redefinirSenha">
        <div class="input-group">
          <label for="nova-senha">Nova Senha</label>
          <input type="password" id="nova-senha" v-model="novaSenha" required />
        </div>
        <div class="input-group">
          <label for="confirmar-senha">Confirmar Nova Senha</label>
          <input type="password" id="confirmar-senha" v-model="confirmarSenha" required />
        </div>

        <button type="submit" :disabled="processando">
          {{ processando ? 'Aguarde...' : 'Salvar Nova Senha' }}
        </button>
        
        <p v-if="mensagem" :class="sucesso ? 'success-message' : 'error-message'">{{ mensagem }}</p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();

const token = ref('');
const tokenValido = ref(false);
const novaSenha = ref('');
const confirmarSenha = ref('');

const mensagem = ref('');
const sucesso = ref(false);
const processando = ref(false);

// Assim que a página carrega, pegamos o token da URL
onMounted(() => {
  const tokenDaUrl = route.query.token;
  if (tokenDaUrl) {
    token.value = tokenDaUrl;
    tokenValido.value = true;
  }
});

const redefinirSenha = async () => {
  if (novaSenha.value !== confirmarSenha.value) {
    mensagem.value = 'As senhas não coincidem.';
    sucesso.value = false;
    return;
  }

  processando.value = true;
  mensagem.value = '';

  try {
  
    await axios.post('http://localhost:5248/api/conta/redefinir-senha', {
      token: token.value,
      novaSenha: novaSenha.value
    });

    sucesso.value = true;
    mensagem.value = 'Senha redefinida com sucesso! Você será redirecionado para o login.';

    // Redireciona para o login após 3 segundos
    setTimeout(() => {
      router.push('/');
    }, 3000);

  } catch (error) {
    sucesso.value = false;
    mensagem.value = error.response?.data?.message || 'Erro ao redefinir a senha. O token pode ser inválido ou expirado.';
  } finally {
    processando.value = false;
  }
};
</script>

<style scoped>
.reset-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f4f4f4;
  font-family: 'Segoe UI', sans-serif;
}
.reset-form {
  background: white;
  padding: 2rem 3rem;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
}
h2 {
  text-align: center;
  margin-bottom: 1.5rem;
  color: #333;
}
.input-group {
  margin-bottom: 1rem;
}
label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
}
input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
button {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 4px;
  background-color: #f47b20;
  color: white;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
button:hover {
  background-color: #d86713;
}
button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
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
</style>