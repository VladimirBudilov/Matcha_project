import { createRouter, createWebHistory } from 'vue-router'
import HomeVue from '@/views/Home.vue'
import LoginVue from '@/views/Login.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BACK),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeVue
    },
    {
      path: '/login',
      name: 'login',
      component: LoginVue
    }
    //{
    //  path: '/signup',
    //    name: 'Signup',
    //    component: SignupView
    //}
  ]
})

export default router
