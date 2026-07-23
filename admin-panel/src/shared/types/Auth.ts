export type User = {
  id: string;
  email: string;
  fullName: string;
  isAdmin: boolean;
};

export type AuthContextType = {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
};

export type LoginUser = {
  email: string;
  password: string;
};

export interface LoginResponse {
  token: string;
  expiresIn: string; // or Date if you transform it
  user: LoginUserInfo;
}

export interface LoginUserInfo {
  id: string;
  email: string;
  fullName: string;
  isAdmin: boolean;
  isActive: boolean;
  createdAt: string;
}
