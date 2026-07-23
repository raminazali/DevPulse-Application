import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import type {
  GetUsersParams,
  UserListItemDto,
} from "../types/UserListItemDto";
import { userService } from "../services/user.service";
import type { PagedList } from "../types/pagedList";

const PAGE_SIZE = 20;

export function useUsers(params: GetUsersParams) {
  return useQuery({
    queryKey: ["users", params],
    queryFn: () => userService.getUsers(params),
  });
}

export function useUser(id: string) {
  return useQuery({
    queryKey: ["users", id],
    queryFn: () => userService.getUserById(id),
    enabled: !!id,
  });
}

export function useInfiniteUsers(search: string) {
  return useInfiniteQuery({
    queryKey: ["users-infinite", search],
    queryFn: ({ pageParam = 1 }) =>
      userService.getUsers({
        page: pageParam,
        pageSize: PAGE_SIZE,
        search,
      } satisfies GetUsersParams) as Promise<PagedList<UserListItemDto>>,
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}
