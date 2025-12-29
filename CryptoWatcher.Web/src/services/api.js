import axios from 'axios';

const API_URL = 'http://localhost:5065/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar token em todas as requisições
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Auth
export const loginApi = (email, password) => 
  api.post('/auth/login', { email, password });

export const registerApi = (email, password, name) => 
  api.post('/auth/register', { email, password, name });

// Alerts
export const getActiveAlerts = () => 
  api.get('/alerts/active');

export const createAlert = (cryptoSymbol, targetPrice, condition) => 
  api.post('/alerts', { cryptoSymbol, targetPrice, condition });

export default api;