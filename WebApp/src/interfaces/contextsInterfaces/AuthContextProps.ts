import type { ExternalLoginPayload } from '../authInterfaces/ExternalLoginPayload';
import type { LoginPayload } from '../authInterfaces/LoginPayload';
import type { PasswordResetPayload } from '../authInterfaces/PasswordResetPayload';
import type { AuthUser } from '../userInterfaces/AuthUser';

export interface AuthContextProps {
  token: string | null;
  authUser: AuthUser | null;
  handleLogin: (payload: LoginPayload) => Promise<void>;
  handleExternalLogin: (payload: ExternalLoginPayload) => Promise<void>;
  handlePasswordResetRequest: (email: string) => Promise<string>;
  handlePasswordReset: (payload: PasswordResetPayload) => Promise<string>;
  handleLogout: () => void;
}
