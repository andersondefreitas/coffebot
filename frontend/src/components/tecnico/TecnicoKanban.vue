<template>
  <div class="dashboard-container">
    <NavbarTecnico />

    <header class="dashboard-header">
      <div class="header-title">
        <h1>Dashboard do Técnico</h1>
        <p>Gerenciamento de chamados</p>
      </div>
      
      <div class="header-filters">
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="Buscar por título, cliente ou ID..."
          class="search-input"
        />
        </div>
    </header>

    <div v-if="loading" class="loading-message">
      Carregando chamados...
    </div>
    
    <div v-else-if="error" class="error-message">
      Ocorreu um erro ao carregar os chamados: {{ error }}
    </div>

    <main v-else-if="chamados.length > 0" class="kanban-board">
      
      <div 
        v-for="(chamadosNaColuna, nomeColuna) in colunasDoBoard" 
        :key="nomeColuna" 
        class="kanban-column"
      >
        <header class="column-header">
          <h3>{{ nomeColuna }}</h3>
          <span class="chamado-count">{{ chamadosNaColuna.length }}</span>
        </header>

        <div class="card-list">
          <ChamadoCard 
            v-for="chamado in chamadosNaColuna"
            :key="chamado.idChamado"
            :chamado="chamado"
            @verDetalhes="handleVerDetalhes" 
          />

          <div v-if="chamadosNaColuna.length === 0" class="empty-column">
            Nenhum chamado aqui.
          </div>
        </div>
      </div>
    </main>
    
    <div v-else class="no-chamados">
      <p>Nenhum chamado encontrado.</p>
    </div>

    <DetalheChamadoModal
      :chamado="chamadoSelecionado"
      @fechar="fecharModal"
      @chamado-atualizado="handleChamadoAtualizado"
      @chamado-arquivado="handleChamadoArquivado"
    />

  </div>
</template>

<script setup>
// --- MUDANÇA: Imports atualizados ---
import { ref, onMounted, computed, watch } from 'vue';
import axios from 'axios';
// (Removemos 'kanbanColuna' e 'draggable')
import NavbarTecnico from './NavbarTecnico.vue'; 
import ChamadoCard from './ChamadoCardTecnico.vue'; // <-- Importamos o Card de volta
import DetalheChamadoModal from './DetalheChamadoModal.vue'; 

// --- Refs principais (sem alteração) ---
const chamados = ref([]); 
const loading = ref(true);
const error = ref(null);
const searchQuery = ref('');
const chamadoSelecionado = ref(null); 

// --- Lógica do Modal (sem alteração) ---
const handleVerDetalhes = (chamado) => {
  chamadoSelecionado.value = chamado;
};
const fecharModal = () => {
  chamadoSelecionado.value = null; 
};
const handleChamadoAtualizado = (chamadoAtualizado) => {
  const index = chamados.value.findIndex(c => c.idChamado === chamadoAtualizado.idChamado);
  if (index !== -1) {
    chamados.value[index] = chamadoAtualizado;
  }
  // (Não precisamos de 'buildKanbanBoard()' porque o 'computed' abaixo
  //  fará isso automaticamente quando 'chamados.value' mudar)
  fecharModal();
};
const handleChamadoArquivado = (idChamadoArquivado) => {
  const index = chamados.value.findIndex(c => c.idChamado === idChamadoArquivado);
  if (index !== -1) {
    chamados.value.splice(index, 1);
  }
  // (O 'computed' abaixo tratará de reconstruir o board)
  fecharModal();
};

// --- MUDANÇA: Lógica do Board voltou a ser 'computed' ---

// (Definições de colunas e prioridades)
const colunasDefinidas = [ 
  'A definir prioridade',
  'Chamados', 
  'Suporte em andamento',
  'Suporte completo'
];
const prioridadeValor = { 
  'Prioridade: critica': 1,
  'Prioridade: média': 2,
  'Prioridade: baixa': 3,
};

// 1. O primeiro 'computed' apenas filtra os chamados pela pesquisa
const chamadosFiltrados = computed(() => {
  const query = searchQuery.value.toLowerCase().trim();
  if (!query) {
    return chamados.value; 
  }
  return chamados.value.filter(chamado => {
    const titulo = chamado.titulo?.toLowerCase() || '';
    const cliente = (chamado.usuarioAbertura?.nome || '').toLowerCase();
    const id = chamado.idChamado.toString();
    return titulo.includes(query) || cliente.includes(query) || id.includes(query);
  });
});

// 2. O segundo 'computed' (o Kanban) agora depende dos 'chamadosFiltrados'
const colunasDoBoard = computed(() => { 
  const board = {};
  colunasDefinidas.forEach(coluna => { board[coluna] = []; });
  
  const statusPriorizados = ['Prioridade: critica', 'Prioridade: média', 'Prioridade: baixa'];
  
  // Usamos 'chamadosFiltrados.value' (a lista já filtrada)
  for (const chamado of chamadosFiltrados.value) {
    const status = chamado.status;
    if (statusPriorizados.includes(status)) {
      board['Chamados'].push(chamado);
    } 
    else if (board.hasOwnProperty(status)) {
      board[status].push(chamado);
    }
    else { 
      board['A definir prioridade'].push(chamado);
    }
  }
  
  board['Chamados'].sort((a, b) => {
    const valorA = prioridadeValor[a.status] || 99; 
    const valorB = prioridadeValor[b.status] || 99;
    return valorA - valorB;
  });
  
  return board;
});

// --- Lógica da API (Sem alteração) ---
const fetchChamados = async () => {
  loading.value = true;
  error.value = null;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) throw new Error("Token de autenticação não encontrado.");
    const apiUrl = 'http://localhost:5248/api/chamado';
    const response = await axios.get(apiUrl, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    chamados.value = response.data;
    // (Não precisamos de 'buildKanbanBoard()' aqui, o 'computed' trata disso)
  } catch (err) {
    if (err.response) {
      error.value = `Erro: ${err.response.data.mensagem || err.response.statusText}`;
    } else if (err.request) {
      error.value = "Não foi possível conectar ao servidor. Verifique o backend.";
    } else {
      error.value = err.message;
    }
  } finally {
    loading.value = false;
  }
};

// --- MUDANÇA: Funções de Drag and Drop REMOVIDAS ---
// (onChamadoMoved e updateChamadoStatus foram apagadas)

onMounted(() => {
  fetchChamados();
});
</script>

<style scoped>
/* --- MUDANÇA: O CSS das colunas e cartões voltou para aqui ---
   (Vindo do 'kanbancoluna.vue' que vamos apagar)
*/
.dashboard-container { 
  font-family: 'Segoe UI', sans-serif;
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #f8f9fa; 
}
.dashboard-header { 
  padding: 1rem 2rem 0 2rem;
  flex-shrink: 0;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 1rem;
  flex-wrap: wrap;
}
.header-title h1 { margin: 0; }
.header-title p { margin: 0; padding-bottom: 0.25rem; }
.header-filters {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-bottom: 0.25rem;
}
.search-input {
  padding: 8px 12px;
  font-size: 0.9rem;
  border: 1px solid #ccc;
  border-radius: 6px;
  width: 250px; 
  transition: width 0.3s, box-shadow 0.2s;
  background-color: #fff;
}
.search-input:focus {
  outline: none;
  border-color: #fd0d0d;
  box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.2);
  width: 300px; 
}
.kanban-board { 
  display: flex;
  flex-grow: 1;
  overflow-x: auto;
  overflow-y: hidden;
  padding: 1rem 2rem 2rem 2rem;
  gap: 1rem;
}

/* --- ESTILOS QUE VOLTARAM --- */
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
.card-list {
  /* (Não precisamos mais do min-height, pois não há 'drop zone') */
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
/* (Os estilos de 'ghost-card' e 'chamado-card-drag' foram removidos) */

/* --- FIM DOS ESTILOS QUE VOLTARAM --- */

.loading-message, .error-message, .no-chamados { 
  text-align: center;
  padding: 3rem;
  color: #6c757d;
  background-color: #fff;
  border-radius: 8px;
  margin: 2rem;
}
.error-message { color: #dc3545; }
</style>