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
    else {
      await axios.get('api/auth/get-id').then(async (res) => {
        localStorage.setItem('UserId', String(res?.data))
        await GetProfileInfo()
      })
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
      <footer>
        Matcha
      </footer>
    </div>
  <RouterView />
</template>

<style scoped>



header {
  position: fixed;
  top: 0;
  left: 0;
  background-color: var(--color-background-mute);
  z-index: 999;
}

nav {
  position: relative;
  width: 100vw;
  top:0;
  font-size: 25px;
  text-align: left;
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

footer {
  background-color: var(--color-background-mute);
   position: fixed;
   right: 0;
   bottom: 0;
   text-align: center;
   width: 100vw;
   font-size: 19px;
font-weight: bold;
  color: var(--color-text);
  z-index: 999;
 }

</style>
