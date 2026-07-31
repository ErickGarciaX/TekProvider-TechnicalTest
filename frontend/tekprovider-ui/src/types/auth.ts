export interface LoginInput {
  username: string;
  password: string;
}

export interface RegisterInput {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  username: string;
}
