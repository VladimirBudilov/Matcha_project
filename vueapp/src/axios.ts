import axios from 'axios'

axios.defaults.baseURL =  process.env.VUE_APP_BACKEND_URL;
axios.defaults.headers.common['Authorization'] = 'Bearer ' + localStorage.getItem('token')
