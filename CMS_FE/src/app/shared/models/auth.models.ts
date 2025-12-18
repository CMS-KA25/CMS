export interface User {
  id: string;
  email: string;
  name: string;
  phoneNumber?: string;
  profilePictureURL?: string;
  role: RoleType | number;
  isEmailVerified: boolean;
  createdAt: string;
}

export enum RoleType {
  User = 'User',
  Staff = 'Staff',
  Doctor = 'Doctor',
  Admin = 'Admin',
  Patient = 'Patient'
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface SignUpRequest {
  email: string;
  password: string;
  name: string;
  phoneNumber?: string;
  profileImage?: File;
}

export interface InviteUserRequest {
  email: string;
  name?: string;
  phoneNumber?: string;
  role: RoleType;
}

export interface LoginResponse {
  user: User;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors?: string[];
}

export interface VerifyOtpRequest {
  email: string;
  code: string;
  purpose: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}