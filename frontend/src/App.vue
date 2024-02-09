<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Test from './components/Test.vue';

const test = 0;
</script>

<template>
    <div class="Test" v-if="test">
      <Test/>
    </div>
    <div class="Main" v-else>
      <header>

      </header>
      <div class="Body">
        <nav>
          <RouterLink to="/">Home</RouterLink>
        </nav>
      </div>
      <footer>

      </footer>
    </div>
  <RouterView />


</template>

<style scoped>

</style>
