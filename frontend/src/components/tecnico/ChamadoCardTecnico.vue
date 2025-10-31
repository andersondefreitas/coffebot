<template>
  <div class="chamado-card" :class="priorityClass">
    <div class="card-header">
      <h3>{{ chamado.titulo }}</h3>
    </div>

    <div class="card-body">
      <div class="info-row">
        <strong>Cliente:</strong>
        <span>{{ chamado.usuarioAbertura?.nome || 'N/A' }}</span>
      </div>

      <div class="info-row">
        <strong>Técnico:</strong>
        <span>{{ chamado.tecnicoResponsavel?.usuario?.nome || 'Não atribuído' }}</span>
      </div>

      <div class="info-row">
        <strong>ID:</strong>
        <span>#{{ chamado.idChamado }}</span>
      </div>
      <div class="info-row">
        <strong>Abertura:</strong>
        <span>{{ formatarData(chamado.dataAbertura) }}</span>
      </div>
    </div>

    <div class="card-footer">
      <button 
        class="action-button"
        @click="$emit('verDetalhes', chamado)" 
      >
        Ver Detalhes
      </button>
    </div>
  </div>
</template>

<script setup>
// --- MUDANÇA: Importamos 'computed' ---
import { computed } from 'vue';

// 1. Definimos e recebemos as 'props'
const props = defineProps({
  chamado: {
    type: Object,
    required: true
  }
});

// 2. Definimos os eventos
defineEmits(['verDetalhes']);

// --- MUDANÇA: Lógica das Cores (Computed Property) ---
const priorityClass = computed(() => {
  // Usamos 'prioridade' (do C#) e convertemos para minúsculas
  // O '?' previne erros se a prioridade for nula
  const prioridade = props.chamado.prioridade?.toLowerCase();

  if (prioridade === 'critica') {
    return 'priority-critica';
  }
  if (prioridade === 'média') {
    return 'priority-media';
  }
  if (prioridade === 'baixa') {
    return 'priority-baixa';
  }
  // Se for "A definir" ou qualquer outra coisa
  return 'priority-default';
});


// 3. Função auxiliar para formatar a data
const formatarData = (dataString) => {
  if (!dataString) return 'N/A';
  return new Date(dataString).toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
</script>

<style scoped>
.chamado-card {
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  padding: 1rem;
  display: flex;
  flex-direction: column;
  transition: box-shadow 0.2s;
  
  /* --- MUDANÇA: Adiciona uma borda base --- */
  border-left: 5px solid transparent; 
}
.chamado-card:hover {
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

/* --- MUDANÇA: Novas Classes de Prioridade ---
*/
.priority-critica {
  border-left-color: #dc3545; /* Vermelho */
}
.priority-media {
  border-left-color: #ffc107; /* Amarelo */
}
.priority-baixa {
  border-left-color: #28a745; /* Verde */
}
.priority-default {
  border-left-color: #ccc; /* Cinza (para "A definir") */
}


.card-header {
  padding-bottom: 0.75rem;
  margin-bottom: 0.75rem;
  border-bottom: 1px solid #f0f0f0;
}
.card-header h3 {
  margin: 0;
  font-size: 1rem;
  font-weight: 600;
  word-break: break-word; 
}
.card-body { flex-grow: 1; }
.info-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: #555;
  margin-bottom: 0.5rem;
  gap: 0.5rem; 
}
.info-row strong { color: #333; flex-shrink: 0; }
.info-row span { text-align: right; word-break: break-all; }

.card-footer {
  margin-top: 0.75rem;
  text-align: right;
}
.action-button {
  background-color: #f47b20; 
  color: white;
  border: none;
  padding: 6px 10px;
  font-size: 0.85rem;
  border-radius: 4px;
  cursor: pointer;
}
</style>