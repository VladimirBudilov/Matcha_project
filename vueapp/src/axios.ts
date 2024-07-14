import axios from 'axios'

axios.defaults.baseURL =  process.env.BASE_URL;
axios.defaults.headers.common['Authorization'] = 'Bearer ' + localStorage.getItem('token')
