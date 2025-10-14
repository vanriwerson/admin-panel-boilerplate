import { useState } from 'react';
import api from '../api';
import type { LoginPayload } from '../types';

export function useAuth() {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token')
  );

  async function login(payload: LoginPayload) {
    const { data } = await api.post('/auth/login', payload);
    localStorage.setItem('token', data.token);
    setToken(data.token);
  }

  function logout() {
    localStorage.removeItem('token');
    setToken(null);
  }

  return { token, login, logout };
}
