import './assets/main.css'
import 'ant-design-vue/dist/reset.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { DatePicker } from 'ant-design-vue';

import App from './App.vue'
import router from './router'
import './axios'

const app = createApp(App)


app.use(createPinia())
app.use(router)
app.use(DatePicker);

app.mount('#app')
