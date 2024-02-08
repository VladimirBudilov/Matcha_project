import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/Data.vue'
import SignupView from '../components/SignUp.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/data',
      name: 'data',
      component: HomeView
    },
    {
      path: '/signup',
        name: 'Signup',
        component: SignupView
    }
  ]
})

export default router
