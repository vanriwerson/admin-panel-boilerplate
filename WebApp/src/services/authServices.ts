import api from '../api';
import type {
  LoginPayload,
  LoginResponse,
  ExternalLoginPayload,
  PasswordResetPayload,
} from '../interfaces';

export async function login(payload: LoginPayload): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/login', payload);
  return data;
}

export async function externalLogin(
  payload: ExternalLoginPayload
): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/external', payload);
  return data;
}

export async function requestPasswordReset(email: string): Promise<string> {
  const { data } = await api.post('/auth/password/request-new', { email });
  return data.message;
}

export async function resetPassword(
  payload: PasswordResetPayload
): Promise<string> {
  const { data } = await api.post('/auth/password/reset', payload);
  return data.message;
}
