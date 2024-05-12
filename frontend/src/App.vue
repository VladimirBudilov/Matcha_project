<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Header from './components/Header.vue';
import { SignUpStore } from '@/stores/SignUpStore';
import { storeToRefs } from 'pinia';
import Login from './views/Login.vue';
import { onMounted, ref } from 'vue';
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
  <RouterView />
</template>

<style scoped>

</style>
