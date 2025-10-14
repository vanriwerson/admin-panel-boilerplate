import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5209',
});

export default api;
