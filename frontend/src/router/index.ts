import { createRouter, createWebHistory } from 'vue-router'
import HomeVue from '@/views/Home.vue'
import LoginVue from '@/views/Login.vue'
import Profile from '@/views/Profile.vue'
import axios from 'axios'
import Settings from '@/views/Settings.vue'

const router = createRouter({
  history: createWebHistory(),
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
    },
    {
      path: '/profile',
      name: 'profile',
      component: Profile
    },
    {
      path: '/settings',
      name: 'settings',
      component: Settings
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
