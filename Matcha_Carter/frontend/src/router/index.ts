import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BACK),
  routes: [
    //{
    //  path: '/data',
    //  name: 'data',
    //  component: HomeView
    //},
    //{
    //  path: '/signup',
    //    name: 'Signup',
    //    component: SignupView
    //}
  ]
})

export default router
