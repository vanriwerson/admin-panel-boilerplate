import type { AuthUser } from '../userInterfaces/AuthUser';

export interface LoginResponse extends AuthUser {
  token: string;
}
