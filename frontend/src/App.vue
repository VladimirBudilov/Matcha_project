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
      <div class="Head">
        <nav>
          <RouterView v-slot="{ Component }">
            <keep-alive>
              <component :is="Component" />
            </keep-alive>
          </RouterView>
        </nav>
      </div>
    </header>
  </div>

  <!--<RouterView />-->


</template>

<style scoped>

</style>
