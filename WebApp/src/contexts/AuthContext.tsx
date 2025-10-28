import { createContext, useState, useEffect, type ReactNode } from 'react';
import type {
  AuthContextType,
  AuthUser,
  ExternalLoginPayload,
  LoginPayload,
  LoginResponse,
  PasswordResetPayload,
} from '../interfaces';
import {
  externalLogin,
  login,
  requestPasswordReset,
  resetPassword,
} from '../services';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token')
  );
  const [authUser, setAuthUser] = useState<AuthUser | null>(null);

  const handleAuthData = (data: LoginResponse) => {
    const userData: AuthUser = {
      id: data.id,
      username: data.username,
      fullName: data.fullName,
      permissions: data.permissions,
    };
    setAuthUser(userData);
    setToken(data.token);
    localStorage.setItem('token', data.token);
    localStorage.setItem('authUser', JSON.stringify(userData));
  };

  async function handleLogin(payload: LoginPayload) {
    const data = await login(payload);
    handleAuthData(data);
  }

  async function handleExternalLogin(payload: ExternalLoginPayload) {
    const data = await externalLogin(payload);
    handleAuthData(data);
  }

  async function handlePasswordResetRequest(email: string): Promise<string> {
    return await requestPasswordReset(email);
  }

  async function handlePasswordReset(
    payload: PasswordResetPayload
  ): Promise<string> {
    return await resetPassword(payload);
  }

  function handleLogout() {
    localStorage.removeItem('token');
    localStorage.removeItem('authUser');
    setToken(null);
    setAuthUser(null);
  }

  useEffect(() => {
    const savedUser = localStorage.getItem('authUser');
    if (savedUser) setAuthUser(JSON.parse(savedUser));
  }, []);

  return (
    <AuthContext.Provider
      value={{
        token,
        authUser,
        handleLogin,
        handleExternalLogin,
        handlePasswordResetRequest,
        handlePasswordReset,
        handleLogout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export default AuthContext;
