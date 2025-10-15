export interface UserCreateDto {
  username: string;
  email: string;
  password: string;
  fullName: string;
}

export interface UserReadDto {
  id: number;
  username: string;
  email: string;
  fullName: string;
  createdAt: string;
  updatedAt: string;
}

export interface LoginPayload {
  identifier: string;
  password: string;
}

export interface ExternalLoginPayload {
  externalToken: string;
}

export interface PaginatedResponse<T> {
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
  data: T[];
}

export type UsersPagination = PaginatedResponse<UserReadDto>;
