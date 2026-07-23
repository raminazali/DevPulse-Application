import type {
  CreateProjectRequest,
  UpdateProjectRequest,
  UseProjectsParams,
} from "../types/ProjectListItemDto";
import { api } from "../utils/api";

export const projectService = {
  async getProjects(params: UseProjectsParams) {
    const res = await api.get(`/projects/user`, { params });
    return res.data;
  },

  async getProjectById(id: string) {
    const res = await api.get(`/projects/${id}`);
    return res.data;
  },

  async createProject(data: CreateProjectRequest) {
    const res = await api.post("/projects", data);
    return res.data;
  },

  async updateProject(data: UpdateProjectRequest) {
    const res = await api.put("/projects", data);
    return res.data;
  },

  async deleteProject(id: string) {
    const res = await api.delete(`/projects/${id}`);
    return res.data;
  },
};
