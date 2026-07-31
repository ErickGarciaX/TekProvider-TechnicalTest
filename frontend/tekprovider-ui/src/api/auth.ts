import { httpClient } from './httpClient';
import type { AuthResponse, LoginInput, RegisterInput } from '../types/auth';

export const authApi = {
  login: (input: LoginInput) => httpClient.post<AuthResponse>('/api/auth/login', input),
  register: (input: RegisterInput) => httpClient.post<AuthResponse>('/api/auth/register', input),
};
