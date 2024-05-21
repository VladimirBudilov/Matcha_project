<template>
  <div class="chat-container">
    <div class="messages">
      <div v-for="(msg, index) in messages" :key="index" class="message">
        <strong>{{ msg.user }}:</strong> {{ msg.text }}
      </div>
    </div>
    <input v-model="message" @keyup.enter="sendMessage" placeholder="Type a message..." />
    <button @click="sendMessage">Send</button>
  </div>
</template>

<script>
import connection from '../services/ChatService.js';

export default {
  data() {
    return {
      messages: [],
      message: '',
      user: 'User' + Math.floor(Math.random() * 100) // Replace with actual user identification logic
    };
  },
  mounted() {
    connection.on("ReceiveMessage", (user, message) => {
      this.messages.push({ user, text: message });
    });
  },
  methods: {
    sendMessage() {
      if (this.message.trim() !== '') {
        connection.invoke("SendMessage", this.user, this.message)
            .catch(err => console.error(err.toString()));
        this.message = '';
      }
    }
  }
};
</script>

<style>
.chat-container {
  display: flex;
  flex-direction: column;
  height: 400px;
  width: 300px;
  border: 1px solid #ccc;
  padding: 10px;
}
.messages {
  flex: 1;
  overflow-y: auto;
  margin-bottom: 10px;
}
.message {
  margin-bottom: 5px;
}
input {
  width: calc(100% - 60px);
  margin-right: 10px;
}
button {
  width: 50px;
}
</style>
