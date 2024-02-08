import axios from 'axios'

axios.defaults.baseURL = process.env.BACK;
axios.defaults.headers.common['Authorization'] = 'Bearer ' + localStorage.getItem('token')
