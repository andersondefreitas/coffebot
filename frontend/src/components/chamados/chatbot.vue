<template>
  <div class="chatbot-container">
    <div class="messages-area">
      <div 
        v-for="(msg, index) in messages" 
        :key="index" 
        class="message" 
        :class="{ 'user-message': msg.sender === 'user', 'bot-message': msg.sender === 'bot' }"
      >
        <p>{{ msg.text }}</p>
      </div>
      <div v-if="isLoading" class="message bot-message">
        <p>Coffeebot está digitando...</p>
      </div>
    </div>
    <form @submit.prevent="sendMessage" class="input-area">
      <input 
        v-model="newMessage"
        type="text" 
        placeholder="Digite sua dúvida técnica..."
        :disabled="isLoading"
      />
      <button type="submit" :disabled="isLoading">Enviar</button>
    </form>
  </div>
</template>




<script setup>
import { ref } from 'vue';
import axios from 'axios';

const messages = ref([]); 
const newMessage = ref(''); 
const isLoading = ref(false);

const sendMessage = async () => {
  if (!newMessage.value.trim() || isLoading.value) return;

  const userMessageText = newMessage.value;
  messages.value.push({ sender: 'user', text: userMessageText });
  
  newMessage.value = '';
  isLoading.value = true;

  try {
   
    const apiUrl = 'http://localhost:5248/api/Chat';

    const response = await axios.post(apiUrl, {
      mensagem: userMessageText 
    });
    
   
    const botReply = response.data.resposta;
    
    messages.value.push({ sender: 'bot', text: botReply });

  } catch (error) {
    console.error("Erro ao contatar a API:", error);
    messages.value.push({ 
      sender: 'bot', 
      text: 'Desculpe, não consegui obter uma resposta do servidor.' 
    });
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>

.chatbot-container {
  width: 400px;
  height: 600px;
  border: 1px solid #ccc;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  font-family: sans-serif;
  margin: 20px auto;
  box-shadow: 0 4px 10px rgba(0,0,0,0.1);
}

.messages-area {
  flex-grow: 1;
  padding: 20px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.message {
  padding: 10px 15px;
  border-radius: 18px;
  max-width: 80%;
}

.user-message {
  background-color: #007bff;
  color: white;
  align-self: flex-end;
  border-bottom-right-radius: 4px;
}

.bot-message {
  background-color: #e9e9eb;
  color: black;
  align-self: flex-start;
  border-bottom-left-radius: 4px;
}

.input-area {
  display: flex;
  padding: 10px;
  border-top: 1px solid #ccc;
}

.input-area input {
  flex-grow: 1;
  border: 1px solid #ddd;
  padding: 10px;
  border-radius: 18px;
  margin-right: 10px;
}

.input-area button {
  padding: 10px 20px;
  border: none;
  background-color: #007bff;
  color: white;
  border-radius: 18px;
  cursor: pointer;
}

.input-area button:disabled {
  background-color: #a0a0a0;
  cursor: not-allowed;
}
</style>