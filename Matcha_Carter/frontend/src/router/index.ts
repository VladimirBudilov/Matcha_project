import { createRouter, createWebHistory } from 'vue-router'
import TestVue from '@/components/Test.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BACK),
  routes: [
    {
      path: '/',
      name: 'home',
      component: TestVue
    }
    //{
    //  path: '/signup',
    //    name: 'Signup',
    //    component: SignupView
    //}
  ]
})

export default router
