import { ref } from 'vue'
import { defineStore } from 'pinia'

export const SignUpStore = defineStore('SignUp', () => {
  const IsActiveSignUp = ref(false)
  const IsLogin = ref(localStorage.getItem('token') ? true : false)

  return { IsActiveSignUp, IsLogin }
})
