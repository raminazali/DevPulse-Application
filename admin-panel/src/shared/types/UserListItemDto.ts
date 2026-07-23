export interface UserListItemDto {
  id: string;
  email: string;
  fullName: string;
  isAdmin: boolean;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string | null;
  projectCount: number;
}

export interface GetUsersParams {
  page: number;
  pageSize: number;
  search?: string;
  sortField?: string;
  sortDirection?: "asc" | "desc";
}

/** @deprecated Use GetUsersParams */
export type UsersParam = GetUsersParams;

/** @deprecated Use GetUsersParams */
export type UseUsersParams = GetUsersParams;

export interface CreateUserRequest {
  email: string;
  password: string;
  fullName: string;
}
