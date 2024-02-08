<script setup lang="ts">
import axios from 'axios'
import { onMounted, ref } from 'vue';

const response = ref<Object>()

onMounted(async() => {
  await axios.get('/api/getdata')
  .then((res) => {
    response.value = res
  })
})

</script>

<template>
  <div class="about">
    <h1>This is an about page {{response}}</h1>
  </div>
</template>

<style>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
