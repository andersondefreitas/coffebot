<template>
  <footer class="modal-footer">
    <form @submit.prevent="submeter" class="comentario-form">
      <textarea
        v-model="textoComentario"
        placeholder="Escreva sua resposta aqui..."
        rows="3"
        class="comentario-input"
        :disabled="submitting"
      ></textarea>
      <button type="submit" class="comentario-submit" :disabled="submitting">
        {{ submitting ? 'Enviando...' : 'Enviar Resposta' }}
      </button>
    </form>
  </footer>
</template>

<script setup>
import { ref } from 'vue';

// Este componente recebe o estado 'submitting' (enviando)
defineProps({
  submitting: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['enviar']);
const textoComentario = ref('');

const submeter = () => {
  if (!textoComentario.value.trim()) return;
  
  // Avisa o "Pai" (Modal) que o utilizador quer enviar este texto
  emit('enviar', textoComentario.value);
  
  // Limpa a caixa de texto localmente
  textoComentario.value = '';
}
</script>

<style scoped>
/* Copiámos apenas os estilos do rodapé para aqui */
.modal-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #eee;
  background-color: #fdfdfd;
}
.comentario-form {
  display: flex;
  gap: 0.75rem;
}
.comentario-input {
  flex-grow: 1;
  padding: 0.75rem;
  border: 1px solid #d9d9d9;
  border-radius: 4px;
  font-size: 1rem;
  font-family: 'Segoe UI', sans-serif;
  resize: vertical;
  min-height: 40px;
}
.comentario-submit {
  padding: 0.75rem 1rem;
  border: none;
  background-color: #f47b20;
  color: white;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s;
}
.comentario-submit:hover {
  background-color: #d86713;
}
.comentario-submit:disabled {
  background-color: #aaa;
  cursor: not-allowed;
}
</style>