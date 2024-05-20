import axios from 'axios'

axios.defaults.baseURL = 'https://localhost:5101/';
axios.defaults.headers.common['Authorization'] = 'Bearer ' + localStorage.getItem('token')
