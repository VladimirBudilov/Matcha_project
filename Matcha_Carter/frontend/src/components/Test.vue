<script setup lang="ts">
import axios from 'axios';
import { onMounted, ref } from 'vue';


const test_0 = ref<Object>()

onMounted(async() => {
  await axios.get('/api/getdata').then((res) => {
    test_0.value = res
  })
})

</script>

<template>
  <h1> 1111 </h1>
  <h1> {{ test_0 }} </h1>
</template>

<style scoped>
header {
  line-height: 1.5;
  max-height: 200vh;
}

nav {
  width: 100%;
  font-size: 12px;
  text-align: center;
  margin-top: 2rem;
}

nav a.router-link-exact-active {
  color: var(--color-text);
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  border-left: 1px solid var(--color-border);
}

nav a:first-of-type {
  border: 0;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    position: absolute;
    place-items: top;
    padding-left: 2vw;
    padding-top: 2vh;
    top: 2vh;
    right: 2vw;
  }

  header .Head {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }

  nav {
    text-align: left;
    margin-left: -1rem;
    font-size: 1rem;

    padding: 1rem 0;
    margin-top: 1rem;
  }
}
</style>
