<template>
  <div class="kanban-column">
    <header class="column-header">
      <h3>{{ nomeColuna }}</h3>
      <span class="chamado-count">{{ chamados.length }}</span>
    </header>

    <draggable
      :list="chamados"
      group="chamados"
      itemKey="idChamado"
      class="card-list"
      @change="onDragChange"
    >
      <template #item="{ element: chamado }">
        <ChamadoCard 
          :chamado="chamado"
          :key="chamado.idChamado"
          @verDetalhes="onVerDetalhes"
          class="chamado-card-drag" 
        />
      </template>
      
      <template #footer>
        <div v-if="chamados.length === 0" class="empty-column">
          Nenhum chamado aqui.
        </div>
      </template>
    </draggable>
  </div>
</template>

<script setup>
import draggable from 'vuedraggable';
import ChamadoCard from './ChamadoCardTecnico.vue'; 

// --- CORREÇÃO 1: Atribuímos 'defineProps' a 'props' ---
// Isto é necessário para podermos usar 'props.nomeColuna' abaixo
const props = defineProps({
  nomeColuna: {
    type: String,
    required: true
  },
  chamados: {
    type: Array,
    required: true
  }
});

// 2. EMITS: (Sem alteração)
const emit = defineEmits(['chamado-movido', 'ver-detalhes']);

// 3. FUNÇÕES DE 'REPASSE'
const onDragChange = (event) => {
  // Agora 'props.nomeColuna' vai funcionar
  emit('chamado-movido', event, props.nomeColuna);
}

// --- CORREÇÃO 2: Repassamos o objeto 'chamado' completo ---
// O 'ChamadoCard' envia o objeto 'chamado' completo
const onVerDetalhes = (chamadoObjeto) => {
  // E nós "repassamos" o objeto 'chamado' completo para o 'TecnicoKanban'
  emit('ver-detalhes', chamadoObjeto);
}
</script>

<style scoped>
/* (O seu CSS está 100% correto e permanece o mesmo) */
.kanban-column { 
  flex: 1 1 300px; 
  min-width: 280px; 
  display: flex;
  flex-direction: column;
  background-color: #ebecf0;
  border-radius: 6px;
  max-height: 100%; 
}
.column-header { 
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  flex-shrink: 0;
}
.column-header h3 {
  font-size: 1rem;
  font-weight: 600;
  margin: 0;
}
.chamado-count {
  font-size: 0.9rem;
  font-weight: 600;
  color: #555;
  background-color: #d6d8db;
  padding: 2px 6px;
  border-radius: 12px;
}
.ghost-card {
  opacity: 0.5;
  background: #c8ebfb;
  border: 1px dashed #0d6efd;
}
.chamado-card-drag {
  cursor: grabbing; 
  transition: box-shadow 0.2s;
  user-select: none;
  -webkit-user-select: none; 
}
.card-list {
  min-height: 100px; 
  overflow-y: auto;
  overflow-x: hidden;
  padding: 0 0.75rem 0.5rem 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
.empty-column { 
  font-size: 0.9rem;
  color: #6c757d;
  padding: 1rem;
  text-align: center;
}
</style>