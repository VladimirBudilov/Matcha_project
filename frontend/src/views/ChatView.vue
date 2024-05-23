
<template>
  <div>
    <ChatComponent />
  </div>
</template>

<script>
import ChatComponent from '../components/ChatComponent.vue'; // Adjust the path according to your project structure

export default {
  components: {
    ChatComponent
  }
};
</script>

<style>
body {
  background: white;
  color: black;
}
button {
  color: black; /* Change this to the color you want */
}
</style>
