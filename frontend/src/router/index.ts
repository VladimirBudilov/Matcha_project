import { createRouter, createWebHistory } from 'vue-router'
import HomeVue from '@/views/Home.vue'
import LoginVue from '@/views/Login.vue'
import Profile from '@/views/Profile.vue'
import axios from 'axios'
import Settings from '@/views/Settings.vue'

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
  else {
    await axios.get('/api/auth/get-id')
    .then((res) => {
      if (res?.status === 204) {
        localStorage.removeItem('token')
        localStorage.removeItem('UserId')
        return 'login'
      }
      else {
        localStorage.setItem('UserId', res.data)
      }
    })
  }
})

export default router
