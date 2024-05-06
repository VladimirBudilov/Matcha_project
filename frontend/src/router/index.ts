import { createRouter, createWebHistory } from 'vue-router'
import HomeVue from '@/views/Home.vue'
import LoginVue from '@/views/Login.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
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

router.beforeEach(async (to, from) => {
  let status = 0;
  if (localStorage.getItem('token')) {
    status = 1
  }

  if (status == 0 && to.name !== 'login') {
    return 'login'
  }
  else if (status == 1 && to.name === 'login') {
    return '/'
  }
})

export default router
