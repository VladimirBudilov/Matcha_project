<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Header from './components/Header.vue';
import { SignUpStore } from '@/stores/SignUpStore';
import { storeToRefs } from 'pinia';
import Login from './views/Login.vue';

const IsLogin = storeToRefs(SignUpStore()).IsLogin

</script>

<template>
    <div class="Login" v-if="!IsLogin">
      <Login />
    </div>
    <div class="Main" v-else>
      <div class="Body">
        <Header />
        <nav>
          <RouterLink to="/">Home</RouterLink>
          <RouterLink to="/profile"> {{ firstName }} {{ lastName }}</RouterLink>
          <RouterLink to="/settings">Settings</RouterLink>
          <Logout />
        </nav>
      </div>
    </div>
  <!--<RouterView />-->
</template>

<style scoped>

</style>
