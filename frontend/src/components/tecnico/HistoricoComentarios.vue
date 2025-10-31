<template>
  <div class="comentarios-section">
    <h3>Histórico de Respostas</h3>
    
    <div v-if="loading" class="comentario-loading">
      Carregando histórico...
    </div>
    
    <div v-else-if="erro" class="comentario-erro">
      {{ erro }}
    </div>
    
    <div v-else-if="!comentarios || comentarios.length === 0" class="comentario-vazio">
      Nenhum comentário neste chamado ainda.
    </div>

    <div v-else class="comentarios-list">
      <div 
        v-for="comentario in comentarios" 
        :key="comentario.idComentario"
        class="comentario-item"
        :class="{ 'item-tecnico': comentario.usuarioCriador?.tipoUsuario === 'Tecnico' }"
      >
        <div class="comentario-header">
          <strong>{{ comentario.usuarioCriador?.nome || 'Usuário' }}</strong>
          <span>{{ formatarData(comentario.dataCriacao) }}</span>
        </div>
        <p class="comentario-body">
          {{ comentario.conteudo }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
// Este componente é "burro". Ele apenas recebe os dados.
defineProps({
  comentarios: {
    type: Array,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  },
  erro: {
    type: String,
    default: null
  }
});

// (Função de data copiada)
const formatarData = (dataString) => {
  if (!dataString) return 'N/A';
  return new Date(dataString).toLocaleString('pt-BR', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  });
};
</script>

<style scoped>
/* Copiámos apenas os estilos dos comentários para aqui */
.comentarios-section {
  margin-top: 2rem;
  border-top: 1px solid #eee;
  padding-top: 1.5rem;
}
.comentarios-section h3 {
  margin: 0 0 1rem 0;
  font-size: 1.1rem;
}
.comentario-loading, .comentario-erro, .comentario-vazio {
  text-align: center;
  color: #888;
  padding: 2rem;
}
.comentario-erro {
  color: #dc3545;
}

.comentarios-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.comentario-item {
  background-color: #f8f9fa; /* Cor padrão (Cliente) */
  border-radius: 8px;
  padding: 0.75rem 1rem;
  max-width: 80%;
  align-self: flex-start; /* Alinha à esquerda por padrão */
}
.comentario-item.item-tecnico {
  background-color: #fef3e9; /* Laranja claro (cor do técnico) */
  border: 1px solid #f47b20;
  align-self: flex-end; /* Alinha à direita */
}
.comentario-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}
.comentario-header strong {
  font-size: 0.9rem;
  color: #333;
}
.comentario-header span {
  font-size: 0.8rem;
  color: #888;
}
.comentario-body {
  margin: 0;
  font-size: 1rem;
  line-height: 1.5;
  white-space: pre-wrap;
  word-break: break-word;
}
</style>