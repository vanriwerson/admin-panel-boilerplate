import {
  createContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';
import type {
  AuthContextProps,
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

const AuthContext = createContext<AuthContextProps | undefined>(undefined);
export default AuthContext;

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token')
  );
  const [authUser, setAuthUser] = useState<AuthUser | null>(null);

  const handleAuthData = useCallback((data: LoginResponse) => {
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
  }, []);

  const handleLogin = useCallback(
    async (payload: LoginPayload) => {
      const data = await login(payload);
      handleAuthData(data);
    },
    [handleAuthData]
  );

  const handleExternalLogin = useCallback(
    async (payload: ExternalLoginPayload) => {
      const data = await externalLogin(payload);
      handleAuthData(data);
    },
    [handleAuthData]
  );

  const handlePasswordResetRequest = useCallback(
    async (email: string): Promise<string> => {
      return await requestPasswordReset(email);
    },
    []
  );

  const handlePasswordReset = useCallback(
    async (payload: PasswordResetPayload): Promise<string> => {
      return await resetPassword(payload);
    },
    []
  );

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
