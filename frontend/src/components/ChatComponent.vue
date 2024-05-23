

<template>
<div class="chat">
  <div class="chat-container">
    <div class="messages">
      <div v-for="(msg, index) in messages" :key="index" class="message">
        <strong>{{ msg.user }}:</strong> {{ msg.text }}
      </div>
    </div>
    <input v-model="message" @keyup.enter="sendMessage" placeholder="Type a message..." />
    <button @click="sendMessage">Send</button>
  </div>
  <div class="room-actions">
    <input v-model="roomName" placeholder="Room name..." />
    <button @click="createRoom">Create Room</button>
    <button @click="joinRoom">Join Room</button>
    <button @click="leaveRoom">Leave Room</button>
    <input v-model="inviteeId" placeholder="Invitee connection ID..." />
    <button @click="inviteToRoom">Invite to Room</button>
  </div>
</div>
</template>

<script>
import createConnection from "../services/ChatService.js";

export default {
  data() {
    return {
      connection: null,
      roomName: '',
      messages: [],
      message: '',
      user: 'User' + localStorage.getItem('UserId'),
      inviteeId: ''
    };
  },
  mounted() {
    console.log('mounted')
    this.connection = createConnection();
    console.log("conection", this.connection);
    this.connection.start().catch(err => console.error(err.toString()));
    this.connection.on("ReceiveMessage", (user, message) => {
      this.messages.push({ user, text: message });
    });
  },
  methods: {
    sendMessage() {
      if (this.message.trim() !== '') {
        console.log(this.connection);
        this.connection.invoke("SendMessage", this.roomName, this.message)
            .catch(err => console.error(err.toString()));
        this.message = '';
      }
    },
    createRoom() {
      this.connection.invoke("CreateRoom", this.roomName)
          .catch(err => console.error(err.toString()));
    },
    joinRoom() {
      this.connection.invoke("JoinRoom", this.roomName)
          .catch(err => console.error(err.toString()));
    },
    leaveRoom() {
      this.connection.invoke("LeaveRoom", this.roomName)
          .catch(err => console.error(err.toString()));
    },
    inviteToRoom() {
      this.connection.invoke("InviteToRoom", this.roomName, this.inviteeId)
          .catch(err => console.error(err.toString()));
    }
  },
  beforeUnmount() {
    this.connection.stop();
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
.messages {
  flex: 1;
  height: 300px;
  width: 250px;
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
