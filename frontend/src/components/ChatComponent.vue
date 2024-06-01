

<template>
<div class="chat">
  <div class="chat-container" id="messages">
    <div class="messages">
      <div v-for="(msg, index) in messages" :key="index" class="message">
        <strong>{{ msg.user }}:</strong> {{ msg.text }}
      </div>
    </div>
    <input v-model="message" @keyup.enter="sendMessage" placeholder="Type a message..." />
    <button @click="sendMessage">Send</button>
  </div>
  <div class="room-actions" id="room-actions">
    <input v-model="inviteeId" placeholder="Invitee connection ID..." />
    <button @click="StartChat">Start</button>
    <button @click="LeaveChat">Leave</button>
    <button @click="GetMessages">Get Messages</button>
  </div>
</div>
</template>

<script>
import createConnection from "../services/ChatService.ts";
import axios from "axios";

export default {
  data() {
    return {
      connection: null,
      messages: [],
      message: '',
      user: 'User' + localStorage.getItem('UserId'),
      inviteeId: ''
    };
  },
  mounted() {
    this.connection = createConnection();
    this.connection.start().catch(err => console.error(err.toString()));
    this.connection.on("ReceiveMessage", (user, message) => {
      this.messages.push({ user, text: message });
    });
  },
  methods: {
    sendMessage() {
      if (this.message.trim() !== '') {
        this.connection
            .invoke("SendMessage", Number(this.inviteeId), this.message)
            .catch(err => console.error(err.toString()));
        this.message = '';
      }
    },
    StartChat() {
      this.connection.invoke("StartChat", Number(this.inviteeId))
          .then((data) =>
      {
        console.log(data);
      })
          .catch(err => console.error(err.toString()));
    },
    LeaveChat() {
      this.connection.invoke("LeaveChat", Number(this.inviteeId))
          .then(
              () =>
              {
                console.log('Chat left')

              })
          .catch(err => console.error(err.toString()));
    },
    GetMessages() {
      axios.post('api/actions/chat',  {producerId: Number(localStorage.getItem("UserId")), consumerId: Number(this.inviteeId)})
          .then((res) => {
            console.log(res.data);
            this.messages = res.data;
          })
          .catch(err => console.error(err.toString()));
    }
  },
  beforeUnmount() {
    if (this.connection) {
      this.connection.stop()
          .then(() => console.log('Connection stopped'))
          .catch(err => console.error('Error while stopping connection: ' + err));
    }
  },
  beforeDestroy() {
    if (this.connection) {
      this.connection.stop()
          .then(() => console.log('Connection stopped'))
          .catch(err => console.error('Error while stopping connection: ' + err));
    }
  }
};
</script>

<style>
.chat {
  margin-top: 100px;
  display: flex;
  flex-direction: row; /* Change from column to row */
  justify-content: space-between; /* Adjust this as needed */
  align-items: flex-start; /* Align items to the start of the container */
  height: 500px;
  width: 700px;
}

.chat-container {
  display: flex;
  flex-direction: column;
  height: 400px;
  width: 300px;
  border: 1px solid #ccc;
  padding: 10px;
}

.room-actions {
  display: flex;
  flex-direction: column; /* Change from row to column */
  margin-left: 10px; /* Add left margin */
}

.messagesOutput{
  color: white;
}

#room-actions, #messages{
  color: black
}
.messages {
  flex: 1;
  height: 300px;
  width: 250px;
}
.message {
  margin-bottom: 5px;
  color: white;
}
/*input {
  width: calc(100% - 60px);
  margin-right: 10px;
}
button {
  width: 50px;
}*/
</style>
