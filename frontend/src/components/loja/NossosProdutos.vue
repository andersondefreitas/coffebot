<template>
  <div class="page-container">
    <header class="navbar">
      <div class="nav-left">
        <router-link to="/home" class="logo-link">
          <img :src="logoIcon" alt="Logo Coffeebot" class="logo-icon" />
          <span>COFFEEBOT</span>
        </router-link>
      </div>
      <div class="nav-right">
        <router-link to="/register" class="nav-link">Abra sua conta</router-link>
        <router-link to="/" class="nav-button-cta">Acesse sua conta</router-link>
      </div>
    </header>

    <main class="products-container">
      <div class="products-grid">
        <div v-for="product in products" :key="product.id" class="product-card">
          <div class="product-image-container">
            <img :src="product.image" :alt="product.title" class="product-image" />
          </div>
          
          <h3 class="product-title">{{ product.title }}</h3>
          
          <div class="product-price">
            <span>12x</span>
            <strong>R$ {{ product.price }}</strong>
          </div>
          
          <button 
            class="product-button"
            :class="{ 'added-to-cart': isProductInCart(product.id) }"
            :disabled="isProductInCart(product.id)"
            @click="addToCart(product)"
          >
            {{ isProductInCart(product.id) ? 'Adicionado ✓' : `Pedir ${product.title}` }}
          </button>
          
          <ul class="features-list">
            <li v-for="(feature, index) in product.features" :key="index">
              <i class="material-icons">{{ feature.icon }}</i>
              <span>{{ feature.text }}</span>
            </li>
          </ul>
        </div>
      </div>
    </main>

    <div class="cart-fab" @click="toggleCart">
      <i class="material-icons">shopping_cart</i>
      <span v-if="cartItems.length > 0" class="cart-badge">{{ cartItemCount }}</span>
    </div>

    <aside class="cart-sidebar" :class="{ 'is-open': isCartOpen }">
      <div class="cart-header">
        <h3>Seu Carrinho</h3>
        <button @click="toggleCart" class="close-button">&times;</button>
      </div>

      <div v-if="cartItems.length === 0" class="cart-empty">
        <p>Seu carrinho está vazio.</p>
      </div>

      <ul v-else class="cart-items-list">
        <li v-for="item in cartItems" :key="item.id" class="cart-item">
          <img :src="item.image" :alt="item.title" class="cart-item-image" />
          <div class="cart-item-details">
            <span class="cart-item-title">{{ item.title }}</span>
            <span class="cart-item-price">R$ {{ item.price }}</span>
          </div>
          <button @click="removeFromCart(item.id)" class="remove-button">
            <i class="material-icons">delete</i>
          </button>
        </li>
      </ul>

      <div class="cart-footer">
        <div class="subtotal">
          <span>Subtotal</span>
          <strong>R$ {{ subtotal.toFixed(2).replace('.', ',') }}</strong>
        </div>
      <router-link to="/carrinho" class="checkout-button">Ir para o carrinho</router-link>
      </div>
    </aside>
    <div v-if="isCartOpen" class="overlay" @click="toggleCart"></div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';

// --- CORREÇÃO AQUI ---
// Como este ficheiro está em 'components/loja/', precisamos
// subir 2 níveis ('../../') para encontrar a pasta 'assets'.
import logoIcon from '../../assets/logoCoffeebot.png';
import maquinaSmartImg from '../../assets/maquina-smart.png';
import totemImg from '../../assets/totem.png';
import sistemaCompletoImg from '../../assets/sistema-completo.png';

// Lógica do Carrinho
const cartItems = ref([]);
const isCartOpen = ref(false);

const products = ref([
  { id: 1, title: 'maquina smart', price: '49,90', image: maquinaSmartImg, features: [ { icon: 'wifi', text: 'Conexão wi-fi e chip limitado' }, { icon: 'payment', text: 'Aproximação e link de pagamento' }, { icon: 'system_update_alt', text: 'Sistema integrado gratuitamente por um ano' }, { icon: 'qr_code_2', text: 'Pix QR Code e Pix NFC grátis' }, { icon: 'battery_charging_full', text: 'Bateria recarregável com base carregadora' } ] },
  { id: 2, title: 'totem', price: '99,90', image: totemImg, features: [ { icon: 'wifi', text: 'Conexão wi-fi e chip limitado' }, { icon: 'payment', text: 'Aproximação e link de pagamento' }, { icon: 'system_update_alt', text: 'Sistema integrado gratuitamente por um ano' }, { icon: 'qr_code_2', text: 'Pix QR Code e Pix NFC grátis' }, { icon: 'battery_charging_full', text: 'Bateria recarregável com base carregadora' } ] },
  { id: 3, title: 'sistema completo', price: '139,90', image: sistemaCompletoImg, features: [ { icon: 'wifi', text: 'Conexão wi-fi e chip limitado' }, { icon: 'payment', text: 'Aproximação e link de pagamento' }, { icon: 'system_update_alt', text: 'Sistema integrado gratuitamente por dois anos' }, { icon: 'qr_code_2', text: 'Pix QR Code e Pix NFC grátis' }, { icon: 'battery_charging_full', text: 'Bateria recarregável com base carregadora' } ] }
]);

const addToCart = (product) => {
  if (!cartItems.value.some(item => item.id === product.id)) {
    cartItems.value.push({ ...product, quantity: 1 });
  }
};

const removeFromCart = (productId) => {
  cartItems.value = cartItems.value.filter(item => item.id !== productId);
};

const toggleCart = () => {
  isCartOpen.value = !isCartOpen.value;
};

const isProductInCart = (productId) => {
  return cartItems.value.some(item => item.id === productId);
};

const cartItemCount = computed(() => {
  return cartItems.value.length;
});

const subtotal = computed(() => {
  return cartItems.value.reduce((total, item) => {
    const price = parseFloat(item.price.replace(',', '.'));
    return total + price;
  }, 0);
});
</script>

<style scoped>
.page-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f8f9fa;
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

.nav-left .logo-link { display: flex; align-items: center; text-decoration: none; color: #333; font-weight: bold; font-size: 1.5rem; }
.logo-icon { height: 32px; margin-right: 0.5rem; }
.nav-right { display: flex; align-items: center; gap: 1.5rem; }
.nav-link { text-decoration: none; color: #555; font-weight: 600; }
.nav-button-cta { background-color: #f47b20; color: white; padding: 0.75rem 1.5rem; border-radius: 25px; font-weight: bold; text-decoration: none; cursor: pointer; transition: background-color 0.3s; }

.products-container { padding: 4rem 2rem; }
.products-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 2rem; max-width: 1200px; margin: 0 auto; }

.product-card {
  background-color: #fff;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  box-shadow: 0 4px 6px rgba(0,0,0,0.05);
  transition: transform 0.2s, box-shadow 0.2s;
}

.product-card:hover { transform: translateY(-5px); box-shadow: 0 8px 15px rgba(0,0,0,0.1); }

.product-image-container { height: 200px; width: 100%; display: flex; align-items: center; justify-content: center; margin-bottom: 1.5rem; }
.product-image { max-width: 100%; max-height: 100%; height: auto; }
.product-title { font-size: 1.25rem; font-weight: 600; text-transform: uppercase; color: #333; margin: 0; }
.product-price { margin: 0.5rem 0 1.5rem 0; font-size: 1.2rem; color: #555; }
.product-price strong { font-size: 2.5rem; font-weight: 700; color: #333; margin-left: 0.25rem; }

.product-button { width: 100%; background-color: #343a40; color: white; border: none; padding: 0.8rem; border-radius: 4px; font-size: 1rem; font-weight: bold; cursor: pointer; transition: background-color 0.2s; }
.product-button:hover { background-color: #23272b; }
.product-button.added-to-cart { background-color: #f47b20; cursor: not-allowed; }

.features-list { list-style: none; padding: 0; margin-top: 2rem; width: 100%; text-align: left; }
.features-list li { display: flex; align-items: center; gap: 0.75rem; margin-bottom: 0.75rem; color: #6c757d; font-size: 0.9rem; }
.features-list .material-icons { color: #f47b20; font-size: 1.5rem; }

/* ESTILOS DO CARRINHO */
.cart-fab { position: fixed; bottom: 2rem; right: 2rem; background-color: #343a40; color: white; width: 60px; height: 60px; border-radius: 50%; display: flex; align-items: center; justify-content: center; box-shadow: 0 4px 10px rgba(0,0,0,0.2); cursor: pointer; transition: transform 0.2s; z-index: 1001; }
.cart-fab:hover { transform: scale(1.1); }

.cart-badge { position: absolute; top: -5px; right: -5px; background-color: #f47b20; color: white; width: 24px; height: 24px; border-radius: 50%; font-size: 0.8rem; font-weight: bold; display: flex; align-items: center; justify-content: center; }

.cart-sidebar { position: fixed; top: 0; left: 0; width: 380px; height: 100%; background-color: #fff; box-shadow: 4px 0 15px rgba(0,0,0,0.1); transform: translateX(-100%); transition: transform 0.3s ease-in-out; z-index: 1002; display: flex; flex-direction: column; }
.cart-sidebar.is-open { transform: translateX(0); }

.cart-header { padding: 1.5rem; border-bottom: 1px solid #eee; display: flex; justify-content: space-between; align-items: center; }
.cart-header h3 { margin: 0; font-size: 1.25rem; }
.close-button { background: none; border: none; font-size: 2rem; cursor: pointer; color: #888; }

.cart-empty { flex-grow: 1; display: flex; align-items: center; justify-content: center; color: #888; }
.cart-items-list { list-style: none; padding: 1rem; margin: 0; overflow-y: auto; flex-grow: 1; }
.cart-item { display: flex; align-items: center; gap: 1rem; padding: 1rem 0; border-bottom: 1px solid #f0f0f0; }
.cart-item-image { width: 60px; height: 60px; object-fit: contain; }
.cart-item-details { display: flex; flex-direction: column; flex-grow: 1; }
.cart-item-title { font-weight: 600; text-transform: capitalize; }
.cart-item-price { color: #666; }
.remove-button { background: none; border: none; color: #aaa; cursor: pointer; }
.remove-button:hover { color: #d93025; }

.cart-footer { padding: 1.5rem; border-top: 1px solid #eee; }
.subtotal { display: flex; justify-content: space-between; font-size: 1.1rem; margin-bottom: 1rem; }
.checkout-button { width: 100%; padding: 1rem; background-color: #f47b20; color: white; border: none; border-radius: 4px; font-size: 1.1rem; font-weight: bold; cursor: pointer; }

.overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background-color: rgba(0,0,0,0.4); z-index: 1000; }
</style>