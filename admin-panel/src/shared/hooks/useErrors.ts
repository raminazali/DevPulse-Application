import { useQuery } from "@tanstack/react-query";
import { errorsService } from "../services/errors.service";
import type { ErrorParameters } from "../types/errorsType";

export function useErrors(params: ErrorParameters) {
  return useQuery({
    queryKey: ["errors", params],
    queryFn: () => errorsService.getErrors(params),
  });
}
export function useError(id: string) {
  return useQuery({
    queryKey: ["errors", id],
    queryFn: () => errorsService.getErrorById(id),
    enabled: !!id,
  });
}
