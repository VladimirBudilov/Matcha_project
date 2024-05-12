<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Logout from '@/components/Logout.vue'
import { SignUpStore } from '@/stores/SignUpStore';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch } from 'vue';
import axios from 'axios';

const IsLogin = storeToRefs(SignUpStore()).IsLogin
const firstName = ref('')
const lastName = ref('')

async function GetProfileInfo(){
  await axios.get('api/profile/' + localStorage.getItem('UserId')).then((res) => {
      firstName.value = res.data.firstName
      lastName.value = res.data.lastName
    })
}

onMounted (async () => {
  setTimeout(async () => {
    if (localStorage.getItem('UserId')) {
      await GetProfileInfo()
    }
  }, 100)

})

</script>

<template>
    <div class="Main" v-if="IsLogin">
      <header>
        <nav>
          <RouterLink to="/">Home</RouterLink>
          <RouterLink to="/profile"> {{ firstName }} {{ lastName }}</RouterLink>
          <RouterLink to="/settings">Settings</RouterLink>
          <Logout />
        </nav>
      </header>
    </div>
  <RouterView />
</template>

<style scoped>
header {
  position: relative;
  width: 98vw;
  background-color: var(--color-border);
  margin: 1vh 1vw;
}

nav {
  position: relative;
  width: 100vw;
  font-size: 25px;
  text-align: left;
  margin-top: 2vh;
}

nav a.router-link-exact-active {
  color: var(--color-text);
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 1rex 1rem;
  border-left: 1px solid var(--color-border);
}

nav a:first-of-type {
  border: 0;
}

</style>
