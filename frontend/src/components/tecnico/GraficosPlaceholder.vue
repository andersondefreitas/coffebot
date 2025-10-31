<template>
  <div class="placeholder-container">
    <NavbarTecnico />
    <div class="placeholder-content">
      <h1>Página de Dashboards</h1>
      <p>Esta página está em construção.</p>
      <router-link to="/tecnico/dashboard">Voltar ao Kanban</router-link>
    </div>
  </div>
</template>

<script setup>
import NavbarTecnico from './NavbarTecnico.vue';
</script>

<style scoped>
.placeholder-content {
  text-align: center;
  padding-top: 5rem;
  font-family: 'Segoe UI', sans-serif;
}
</style>