<template>
  <div class="page-container">
    <header class="navbar">
      <div class="nav-left">
        <router-link to="/home" class="logo-link">
          <span>COFFEEBOT</span>
        </router-link>
      </div>
      <div class="nav-right">
        <router-link to="/produtos" class="nav-link">Continuar comprando</router-link>
      </div>
    </header>

    <main class="cart-content">
      <h1>Meu Carrinho</h1>
      <p>Aqui você verá os itens do seu carrinho e poderá finalizar a compra.</p>
      <p><em>(Página em construção)</em></p>
      
      <div class="info-box">
        <strong>Nota para o desenvolvedor:</strong> Os itens adicionados na página de produtos não aparecem aqui ainda. Para isso, o próximo passo é usar um gerenciador de estado global como o Pinia.
      </div>
    </main>
  </div>
</template>

<script setup>
</script>

<style scoped>
.page-container {
  font-family: 'Segoe UI', sans-serif;
}
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 3rem;
  background-color: #fff;
  border-bottom: 1px solid #eee;
}
.nav-left .logo-link { text-decoration: none; color: #333; font-weight: bold; font-size: 1.5rem; }
.nav-link { text-decoration: none; font-weight: 600; color: #f47b20; }
.cart-content {
  text-align: center;
  padding: 4rem 2rem;
}
.info-box {
  margin: 2rem auto;
  padding: 1rem;
  max-width: 600px;
  background-color: #fffbe6;
  border: 1px solid #ffe58f;
  border-radius: 8px;
  text-align: left;
}
</style>