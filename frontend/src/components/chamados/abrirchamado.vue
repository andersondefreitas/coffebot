<template>
  <div class="page-container">
    <header class="main-header">
      <div class="logo">
        <img :src="logoIcon" alt="Coffeebot Icon" />
        <span>COFFEEBOT</span>
      </div>
      <nav class="main-nav">
        <RouterLink to="/chatbot-ia">assistente virtual</RouterLink>
      </nav>
      <div class="user-profile">
        <img :src="userIcon" alt="User Icon" />
      </div>
    </header>

    <main class="content-area">
      <form @submit.prevent="handleSubmit" class="chamado-form">
        <button type="button" class="new-ticket-button">+ fazer um novo chamado</button>

        <div class="form-group">
          <label for="assunto">assunto:</label>
          <input type="text" id="assunto" v-model="assunto" />
        </div>

        <div class="form-group">
          <label for="descricao">descrição:</label>
          <textarea id="descricao" v-model="descricao" rows="6"></textarea>
        </div>

        <p v-if="mensagemErro" class="error-message">
          {{ mensagemErro }}
        </p>

        <div class="form-actions">
          <button type="button" @click="handleCancel" class="btn btn-secondary">CANCELAR CHAMADO</button>
          <button type="submit" class="btn btn-primary">ABRIR CHAMADO</button>
        </div>
      </form>
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter, RouterLink } from 'vue-router';
import axios from 'axios'; // 1. Importamos o axios

// --- Caminhos das imagens (já corrigidos) ---
import logoIcon from '../../assets/logoCoffeebot.png';
import userIcon from '../../assets/iconeUsuario.png';

// --- Variáveis reativas ---
const assunto = ref('');
const descricao = ref('');
const router = useRouter();
const mensagemErro = ref(''); // Para mostrar feedback de erro

// --- FUNÇÃO DE SUBMISSÃO CORRETA (QUE FALA COM O SEU C#) ---
const handleSubmit = async () => {
  mensagemErro.value = ''; // Limpa erros antigos

  // 1. Validar se os campos não estão vazios
  if (!assunto.value || !descricao.value) {
    mensagemErro.value = "Por favor, preencha o assunto e a descrição.";
    return;
  }

  try {
    // 2. Pegar o token de autenticação (ESSENCIAL)
    const token = localStorage.getItem('authToken');
    if (!token) {
      throw new Error("Utilizador não autenticado. Faça login novamente.");
    }

    // 3. Montar os dados (com 'titulo', como o C# espera)
    const dadosChamado = {
      titulo: assunto.value,
      descricao: descricao.value
    };

    // 4. Fazer a chamada POST (para '/api/chamado', singular)
    await axios.post('http://localhost:5248/api/chamado', dadosChamado, {
      headers: {
        'Authorization': `Bearer ${token}` // Envia o token
      }
    });

    // 5. Sucesso!
    alert('Chamado aberto com sucesso!');
    router.push('/home'); // Volta para a home (ou outra página de "sucesso")

  } catch (error) {
    // 6. Tratar erros
    if (error.response) {
      // Erro vindo da API (ex: 401 Unauthorized, 400 Bad Request)
      mensagemErro.value = error.response.data.mensagem || "Erro ao abrir chamado.";
    } else if (error.message === "Utilizador não autenticado...") {
      mensagemErro.value = error.message;
      router.push('/'); // Envia para o login
    } else {
      // Erro de rede ou o backend está offline
      mensagemErro.value = "Não foi possível conectar ao servidor.";
    }
  }
};

// Função de Cancelar (sem alteração)
const handleCancel = () => {
  router.go(-1);
};
</script>

<style scoped>
.page-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f0f2f5;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
.main-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background-color: #fff;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}
.logo {
  display: flex;
  align-items: center;
  font-weight: bold;
  font-size: 1.5rem;
}
.logo img {
  width: 40px;
  margin-right: 10px;
}
.main-nav a {
  text-decoration: none;
  color: #333;
  font-weight: 500;
  font-size: 1.2rem;
}
.user-profile img {
  width: 40px;
  cursor: pointer;
}
.content-area {
  display: flex;
  justify-content: center;
  padding-top: 3rem;
}
.chamado-form {
  background-color: #fff;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 600px;
}
.new-ticket-button {
  background: #e9e9eb;
  border: 1px dashed #ccc;
  border-radius: 5px;
  padding: 0.75rem 1rem;
  font-weight: 500;
  cursor: pointer;
  margin-bottom: 2rem;
  width: auto;
}
.form-group {
  margin-bottom: 1.5rem;
}
.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
  font-size: 0.9rem;
}
.form-group input,
.form-group textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d9d9d9;
  border-radius: 4px;
  font-size: 1rem;
  box-sizing: border-box;
}

/* ADICIONADO: Estilo para a mensagem de erro */
.error-message {
  color: #dc3545; /* Vermelho */
  text-align: center;
  margin-bottom: 1rem;
  font-size: 0.9rem;
  font-weight: 500;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
}
.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  border: none;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.2s ease;
}
.btn-primary {
  background-color: #f47b20;
  color: white;
}
.btn-primary:hover {
  background-color: #d86713;
}
.btn-secondary {
  background-color: #fff;
  color: #f47b20;
  border: 1px solid #f47b20;
}
.btn-secondary:hover {
  background-color: #fef3e9;
}
</style>