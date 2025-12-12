import type {
  PaginatedResponse,
  SystemResource,
  UserFormValues,
} from '../interfaces';

interface FormStates {
  userForm: UserFormValues;
  systemResource: SystemResource;
  tablePagination: Omit<PaginatedResponse<unknown>, 'data'>;
}

export const cleanStates: FormStates = {
  userForm: {
    username: '',
    email: '',
    fullName: '',
    password: '',
    permissions: [],
  },
  systemResource: {
    name: '',
    exhibitionName: '',
  },
  tablePagination: {
    totalItems: 0,
    page: 1,
    pageSize: 10,
    totalPages: 1,
  },
};
