<template>
  <div v-if="chamado" class="modal-overlay" @click.self="fechar">
    <div class="modal-content">
      
      <header class="modal-header">
        <h3>{{ chamado.titulo }}</h3>
        <button @click="fechar" class="close-button">&times;</button>
      </header>

      <main class="modal-body">
        
        <section v-if="chamado.status === 'A definir prioridade'" class="prioridade-section">
          <h4>Definir Prioridade do Chamado:</h4>
          <div class="prioridade-botoes">
            <button @click="definirPrioridade('critica')" :disabled="isSubmitting" class="btn-prioridade critica">Crítica</button>
            <button @click="definirPrioridade('média')" :disabled="isSubmitting" class="btn-prioridade media">Média</button>
            <button @click="definirPrioridade('baixa')" :disabled="isSubmitting" class="btn-prioridade baixa">Baixa</button>
          </div>
        </section>

        <section class="acoes-section" v-if="chamado.status !== 'A definir prioridade'">
          <h4>Ações Rápidas</h4>
          
          <button
            v-if="chamado.status.startsWith('Prioridade:') && !chamado.tecnicoResponsavel"
            @click="assumirChamado"
            :disabled="isSubmitting"
            class="btn-acao assumir">
            <i class="material-icons">person_add</i>
            Assumir Chamado
          </button>

          <button 
            v-else-if="chamado.status === 'Suporte em andamento'"
            @click="mudarStatusDoChamado('Suporte completo')" 
            :disabled="isSubmitting" 
            class="btn-acao encerrar">
            <i class="material-icons">check_circle</i>
            Encerrar Chamado
          </button>
          
          <div v-else-if="chamado.status === 'Suporte completo'" class="botoes-completo">
            <button 
              @click="mudarStatusDoChamado('Suporte em andamento')" 
              :disabled="isSubmitting" 
              class="btn-acao reabrir">
              <i class="material-icons">replay</i>
              Reabrir Chamado
            </button>
            
            <button 
              @click="mudarStatusDoChamado('Arquivado')" 
              :disabled="isSubmitting" 
              class="btn-acao arquivar">
              <i class="material-icons">archive</i>
              Arquivar Chamado
            </button>
          </div>
        </section>

        <div class="detail-grid">
          <div class="detail-group"><strong>Status</strong><span>{{ chamado.status }}</span></div>
          <div class="detail-group"><strong>Prioridade</strong><span>{{ chamado.prioridade }}</span></div>
          <div class="detail-group"><strong>Abertura</strong><span>{{ formatarData(chamado.dataAbertura) }}</span></div>
          <div class="detail-group"><strong>Fechamento</strong><span>{{ formatarData(chamado.dataFechamento) }}</span></div>
          <div class="detail-group"><strong>Cliente</strong><span>{{ chamado.usuarioAbertura?.nome || 'N/A' }}</span></div>
          <div class="detail-group"><strong>Técnico</strong><span>{{ chamado.tecnicoResponsavel?.usuario?.nome || 'Não atribuído' }}</span></div>
        </div>
        <div class="detail-group description">
          <strong>Descrição</strong>
          <p>{{ chamado.descricao || 'Nenhuma descrição fornecida.' }}</p>
        </div>

        <HistoricoComentarios
          :comentarios="comentarios"
          :loading="loadingComentarios"
          :erro="erroComentarios"
        />

      </main>

      <FormularioComentario
        :submitting="isSubmitting"
        @enviar="enviarComentario"
      />

    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import axios from 'axios';
// --- MUDANÇA: Importamos os 2 novos filhos ---
import HistoricoComentarios from './HistoricoComentarios.vue';
import FormularioComentario from './FormularioComentario.vue';

const props = defineProps({
  chamado: {
    type: Object,
    default: null
  }
});

// O 'emit' agora também avisa o pai quando um chamado é arquivado
const emit = defineEmits(['fechar', 'chamado-atualizado', 'chamado-arquivado']);

// --- A lógica "cérebro" (API) permanece aqui ---
const comentarios = ref([]);
const loadingComentarios = ref(false);
const erroComentarios = ref(null);
const isSubmitting = ref(false); 

const fechar = () => { emit('fechar'); }

// --- (Toda a lógica de API permanece neste componente "pai") ---

const definirPrioridade = async (prioridade) => {
  isSubmitting.value = true;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) throw new Error("Não autenticado");
    const dadosPrioridade = { prioridade: prioridade };

    const response = await axios.patch(
      `http://localhost:5248/api/chamado/${props.chamado.idChamado}/prioridade`,
      dadosPrioridade,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    
    emit('chamado-atualizado', response.data);
    fechar(); 

  } catch (err) {
    alert("Erro ao definir prioridade.");
  } finally {
    isSubmitting.value = false;
  }
};

const mudarStatusDoChamado = async (novoStatus) => {
  isSubmitting.value = true;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) throw new Error("Não autenticado");

    await axios.patch(
      `http://localhost:5248/api/chamado/${props.chamado.idChamado}`, 
      { novoStatus: novoStatus }, 
      { headers: { 'Authorization': `Bearer ${token}` } }
    );

    if (novoStatus === 'Arquivado') {
      emit('chamado-arquivado', props.chamado.idChamado);
    } else {
      const chamadoAtualizado = { 
        ...props.chamado, 
        status: novoStatus,
        dataFechamento: (novoStatus === 'Suporte completo') ? new Date().toISOString() : null 
      };
      emit('chamado-atualizado', chamadoAtualizado);
    }
    
    fechar();

  } catch (err) {
    alert("Erro ao encerrar/reabrir o chamado.");
  } finally {
    isSubmitting.value = false;
  }
}

const assumirChamado = async () => {
  isSubmitting.value = true;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) throw new Error("Não autenticado");

    const response = await axios.patch(
      `http://localhost:5248/api/chamado/${props.chamado.idChamado}/atribuir`, 
      {}, 
      { headers: { 'Authorization': `Bearer ${token}` } }
    );

    emit('chamado-atualizado', response.data);
    fechar();

  } catch (err) {
    alert("Erro ao assumir o chamado. Verifique se você é um técnico válido.");
  } finally {
    isSubmitting.value = false;
  }
};

const fetchComentarios = async (chamadoId) => { 
  if (!chamadoId) return;
  loadingComentarios.value = true;
  erroComentarios.value = null;
  comentarios.value = []; 
  try {
    const token = localStorage.getItem('authToken');
    if (!token) throw new Error("Não autenticado");
    const response = await axios.get(
      `http://localhost:5248/api/chamado/${chamadoId}/comentarios`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    comentarios.value = response.data;
  } catch (err) {
    erroComentarios.value = "Não foi possível carregar os comentários.";
  } finally {
    loadingComentarios.value = false;
  }
};

const enviarComentario = async (textoDoFormulario) => { 
  isSubmitting.value = true;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) throw new Error("Não autenticado");
    const dadosComentario = { texto: textoDoFormulario };
    const response = await axios.post(
      `http://localhost:5248/api/chamado/${props.chamado.idChamado}/comentarios`,
      dadosComentario,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    comentarios.value.push(response.data);
  } catch (err) {
    alert("Erro ao enviar comentário.");
  } finally {
    isSubmitting.value = false;
  }
};

watch(() => props.chamado, (novoChamado) => { 
  if (novoChamado) {
    fetchComentarios(novoChamado.idChamado);
  } else {
    comentarios.value = [];
    erroComentarios.value = null;
  }
});

const formatarData = (dataString) => { 
  if (!dataString) return 'N/A';
  return new Date(dataString).toLocaleString('pt-BR', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  });
};
</script>

<style scoped>
/* O CSS do Modal "Pai" fica muito mais limpo! */
.modal-overlay {
  position: fixed; top: 0; left: 0; width: 100vw; height: 100vh;
  background-color: rgba(0, 0, 0, 0.5); display: flex;
  justify-content: center; align-items: center; z-index: 1000;
  transition: opacity 0.2s ease; opacity: 1;
}
.modal-content {
  background-color: white; border-radius: 8px; width: 100%;
  max-width: 700px; max-height: 90vh; display: flex;
  flex-direction: column; overflow: hidden;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
  transform: scale(1); transition: transform 0.2s ease;
}
.modal-header {
  display: flex; justify-content: space-between; align-items: center;
  padding: 1rem 1.5rem; border-bottom: 1px solid #eee;
}
.modal-header h3 { margin: 0; font-size: 1.25rem; }
.close-button {
  background: none; border: none; font-size: 2rem; font-weight: 300;
  line-height: 1; cursor: pointer; color: #888;
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
  flex-grow: 1; 
}

/* Estilos dos detalhes (sem alteração) */
.detail-grid {
  display: grid; grid-template-columns: repeat(2, 1fr); 
  gap: 1.5rem; margin-bottom: 1.5rem;
}
.detail-group { display: flex; flex-direction: column; gap: 0.25rem; }
.detail-group strong { font-size: 0.85rem; color: #555; text-transform: uppercase; }
.detail-group span { font-size: 1rem; }
.detail-group.description { grid-column: 1 / -1; }
.detail-group p {
  font-size: 1rem; line-height: 1.6; white-space: pre-wrap; 
  margin: 0; background-color: #f8f9fa; padding: 1rem; border-radius: 4px;
}

/* (Estilos da Seção de Prioridade - sem alteração) */
.prioridade-section {
  background-color: #fffbef;
  border: 1px solid #f0ad4e;
  border-radius: 6px;
  padding: 1rem 1.5rem;
  margin-bottom: 1.5rem;
}
.prioridade-section h4 {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: #333;
}
.prioridade-botoes {
  display: flex;
  gap: 1rem;
}
.btn-prioridade {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-prioridade:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.btn-prioridade.critica { background-color: #dc3545; color: white; }
.btn-prioridade.critica:hover:not(:disabled) { background-color: #c82333; }
.btn-prioridade.media { background-color: #ffc107; color: #212529; }
.btn-prioridade.media:hover:not(:disabled) { background-color: #e0a800; }
.btn-prioridade.baixa { background-color: #28a745; color: white; }
.btn-prioridade.baixa:hover:not(:disabled) { background-color: #218838; }

/* (Estilos da Seção de Ações - sem alteração) */
.acoes-section {
  background-color: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 6px;
  padding: 1rem 1.5rem;
  margin-bottom: 1.5rem;
}
.acoes-section h4 {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: #333;
}
.btn-acao {
  width: 100%;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 1rem;
}
.btn-acao:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.botoes-completo {
  display: flex;
  gap: 1rem;
}
.btn-acao.encerrar {
  background-color: #28a745;
  color: white;
}
.btn-acao.encerrar:hover:not(:disabled) {
  background-color: #218838;
}
.btn-acao.reabrir {
  background-color: #0d6efd;
  color: white;
  flex: 1; 
}
.btn-acao.reabrir:hover:not(:disabled) {
  background-color: #0b5ed7;
}
.btn-acao.arquivar {
  background-color: #6c757d;
  color: white;
  flex: 1; 
}
.btn-acao.arquivar:hover:not(:disabled) {
  background-color: #5a6268;
}
.btn-acao.assumir {
  background-color: #0d6efd; 
  color: white;
}
.btn-acao.assumir:hover:not(:disabled) {
  background-color: #0b5ed7;
}

/* O CSS do 'comentarios-section' foi movido para o componente filho */
</style>