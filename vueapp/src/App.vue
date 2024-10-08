<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Logout from '@/components/Logout.vue'
import { SignUpStore } from '@/stores/SignUpStore';
import { storeToRefs } from 'pinia';
import WebSocketManager from '@/components/WebSocketManager.vue';
import { onMounted, ref, watch } from 'vue';
import axios from 'axios';

const IsLogin = storeToRefs(SignUpStore()).IsLogin
const firstName = ref('')
const lastName = ref('')

async function GetProfileInfo(){
  await axios.get('/api/profiles/' + localStorage.getItem('UserId')).then((res) => {
      firstName.value = res.data.firstName
      lastName.value = res.data.lastName
    })
}

onMounted (async () => {
  setTimeout(async () => {
    if (localStorage.getItem('token')) {
      if (localStorage.getItem('UserId')) {
        await GetProfileInfo()
      }
      else {
        await axios.get('/api/auth/get-id')
        .then(async (res) => {
          localStorage.setItem('UserId', String(res?.data))
          await GetProfileInfo()
      })
    }
    }

  }, 100)

})

</script>

<template>
  <WebSocketManager />
    <div class="Main" v-if="IsLogin">
      <header>
        <nav>
          <RouterLink to="/">Home</RouterLink>
          <RouterLink to="/profiles"> {{ firstName }} {{ lastName }}</RouterLink>
          <RouterLink to="/settings">Settings</RouterLink>
          <RouterLink to="/chat">Chat</RouterLink>
          <RouterLink to="/blacklist">Blacklist</RouterLink>
          <RouterLink to="/views">Views</RouterLink>
          <RouterLink to="/likers">Likers</RouterLink>
          <Logout />
        </nav>
      </header>
      <footer>
        Matcha
      </footer>
    </div>
  <RouterView />
</template>

<style>

</style>
