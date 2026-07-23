export interface ProjectListItemDto {
  id: string;
  userId: string;
  name: string;
  isActive: boolean;
  createdAt: string;
  errorCount: number;
}

export interface CreateProjectRequest {
  userId: string;
  name: string;
}

export interface UpdateProjectRequest {
  id: string;
  name: string;
  isActive: boolean;
}

export type UseProjectsParams = {
  page: number;
  pageSize: number;
  search?: string;
  sortField?: string;
  sortDirection?: "asc" | "desc";
};
