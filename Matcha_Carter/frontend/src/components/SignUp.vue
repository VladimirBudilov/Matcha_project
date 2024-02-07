<script setup lang="ts">
import axios from 'axios'
import { onMounted, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus'

const open = () => {
  ElMessageBox.prompt('Please input your e-mail', 'Tip', {
    confirmButtonText: 'OK',
    cancelButtonText: 'Cancel',
    inputPattern:
      /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/,
    inputErrorMessage: 'Invalid Email',
  })
    .then(({ value }) => {
      ElMessage({
        type: 'success',
        message: `Your email is:${value}`,
      })
    })
    .catch(() => {
      ElMessage({
        type: 'info',
        message: 'Input canceled',
      })
    })
}

</script>

<template>
  <div class="SignUp">
    <el-button plain @click="open">Sign up</el-button>
  </div>
</template>

<style>
@media (min-width: 1024px) {
  .SignUp {
    min-height: 5vh;
    min-width: 10vw;
    display: flex;
    align-items: center;
  }
}

.el-overlay .is-message-box{
  line-height: 1.5;
  max-height: 200vh;
}

</style>
