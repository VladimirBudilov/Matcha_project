import { createRouter, createWebHistory } from 'vue-router'
import HomeVue from '@/views/Home.vue'
import LoginVue from '@/views/Login.vue'
import Profile from '@/views/Profile.vue'
import Settings from '@/views/Settings.vue'
import Chat from '@/views/Chat.vue'
import User from '@/views/User.vue'
import Block from '@/views/Block.vue'
import Views from '@/views/Views.vue'
import Likers from '@/views/Likers.vue'
import axios from 'axios'


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
      path: '/profiles',
      name: 'profiles',
      component: Profile
    },
    {
      path: '/users/:id',
      name: 'users',
      component: User
    },
    {
      path: '/settings',
      name: 'settings',
      component: Settings
    },
    {
      path: "/chat",
      name: "chat",
      component: Chat
    },
    {
      path: "/blacklist",
      name: "blacklist",
      component: Block
    },
    {
      path: "/views",
      name: "views",
      component: Views
    },
    {
      path: "/likers",
      name: "likers",
      component: Likers
    }
  ]
})

router.beforeEach(async (to, from) => {
  let status = 0;
  await axios.get('/api/auth/get-id').catch(() => {
    localStorage.removeItem('token')
    localStorage.removeItem('UserId')
  })

  if (localStorage.getItem('token')) {
    status = 1
  }

  if (status == 0 && to.name !== 'login') {
    return 'login'
  }
  else if (status == 1 && to.name === 'login') {
    return '/'
  }
});

export default router
