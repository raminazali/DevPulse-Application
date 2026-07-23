import { useQuery } from "@tanstack/react-query";
import { projectService } from "../services/project.service";

type Params = {
  page: number;
  pageSize: number;
  search?: string;
  sortField?: string;
  sortDirection?: "asc" | "desc";
};

export function useProjects(params: Params) {
  return useQuery({
    queryKey: ["projects", params],
    queryFn: () => projectService.getProjects(params),
  });
}

export function useProject(id: string) {
  return useQuery({
    queryKey: ["projects", id],
    queryFn: () => projectService.getProjectById(id),
    enabled: !!id,
  });
}
