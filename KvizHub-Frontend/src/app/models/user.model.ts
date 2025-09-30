export interface User {
  id: number;
  username: string;
  email: string;
  profileImage?: string;
  createdAt: Date;
}

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  profileImage?: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  email: string;
  profileImage?: string;
  token: string;
  createdAt?: Date;
}