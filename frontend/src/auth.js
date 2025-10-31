// Local: src/auth.js

import { ref } from 'vue';

export const isLoggedIn = ref(!!localStorage.getItem('authToken'));

a
export function login(token, router) {
  localStorage.setItem('authToken', token);
  isLoggedIn.value = true;
  router.push('/home');
}


export function logout(router) {
  localStorage.removeItem('authToken');
  isLoggedIn.value = false;
  router.push('/');
}