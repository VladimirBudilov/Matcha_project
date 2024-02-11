import { ref } from 'vue'
import { defineStore } from 'pinia'

export const SignUpStore = defineStore('SignUp', () => {
  const IsActiveSignUp = ref(false)

  return { IsActiveSignUp }
})
