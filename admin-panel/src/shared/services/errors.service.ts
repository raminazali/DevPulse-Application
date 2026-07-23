import type {
  ErrorDetail,
  ErrorGroup,
  ErrorParameters,
} from "../types/errorsType";
import type { PagedList } from "../types/pagedList";
import { api } from "../utils/api";

export const errorsService = {
  async getErrors(params: ErrorParameters) {
    const res = await api.get<PagedList<ErrorGroup>>(`/errors`, {
      params,
    });
    return res.data;
  },

  async getErrorById(id: string) {
    const res = await api.get<ErrorDetail>(`/errors/${id}`);
    return res.data;
  },
};
