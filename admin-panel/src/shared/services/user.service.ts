import type {
  CreateUserRequest,
  GetUsersParams,
} from "../types/UserListItemDto";
import { api } from "../utils/api";

export const userService = {
  async getUsers(params: GetUsersParams) {
    const res = await api.get("/users", { params });
    return res.data;
  },

  async getUserById(id: string) {
    const res = await api.get(`/users/${id}`);
    return res.data;
  },

  async createUser(data: CreateUserRequest) {
    const res = await api.post("/users", data);
    return res.data;
  },
};
