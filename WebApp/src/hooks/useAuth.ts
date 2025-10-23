import { useState } from 'react';
import api from '../api';
import type {
  AuthUser,
  ExternalLoginPayload,
  LoginPayload,
  LoginResponse,
} from '../interfaces';

export function useAuth() {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token')
  );
  const [authUser, setAuthUser] = useState<AuthUser | null>(null);

  const handleAuthData = (data: LoginResponse) => {
    setAuthUser({
      username: data.username,
      fullName: data.fullName,
      permissions: data.permissions,
    });
    setToken(data.token);
  };

  async function login(payload: LoginPayload) {
    const { data } = await api.post('/auth/login', payload);
    localStorage.setItem('token', data.token);

    handleAuthData(data);
  }

  async function externalLogin(payload: ExternalLoginPayload) {
    const { data } = await api.post('/auth/external', payload);
    localStorage.setItem('token', data.token);

    handleAuthData(data);
  }

  function logout() {
    localStorage.removeItem('token');
    setToken(null);
  }

  return { token, authUser, login, externalLogin, logout };
}
