import type { LoginResponse, LoginUser } from "../types";
import { api } from "../utils/api";

export const loginService = {
  async login(params: LoginUser): Promise<LoginResponse> {
    const res = await api.post<LoginResponse>("/Auth/login", params);
    return res.data;
  },
};
