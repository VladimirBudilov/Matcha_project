import { createRouter, createWebHistory } from 'vue-router'
import HomeVue from '@/views/Home.vue'
import LoginVue from '@/views/Login.vue'
import Profile from '@/views/Profile.vue'
import Settings from '@/views/Settings.vue'
import ChatView from '@/views/ChatView.vue'
import Chat from '@/views/Chat.vue'
import User from '@/views/User.vue'


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
      path: '/user/:id',
      name: 'user',
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
      component: ChatView
    },
    {
      path: "/chat2",
      name: "chat2",
      component: Chat
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
})

export default router
