import api from '../../api';
import type { UserRead, UsersPagination } from '../../interfaces';

export async function listUsers(
  pageNumber = 1,
  pageSize = 10,
  searchKey: string = ''
) {
  const defaultParams = `page=${pageNumber}&pageSize=${pageSize}`;

  const endpoint = searchKey
    ? `/users/search?key=${encodeURIComponent(searchKey)}&${defaultParams}`
    : `/users?${defaultParams}`;

  const { data } = await api.get<UsersPagination>(endpoint);
  return data;
}

export async function listUsersForSelect() {
  const { data } = await api.get<UserRead[]>('/users/options');
  return data;
}
