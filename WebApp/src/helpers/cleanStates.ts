import type {
  PaginatedResponse,
  SystemLogFiltersPayload,
  SystemResource,
  UserFormValues,
} from '../interfaces';

interface FormStates {
  userForm: UserFormValues;
  systemResource: SystemResource;
  tablePagination: Omit<PaginatedResponse<unknown>, 'data'>;
  logsReportFilters: Omit<SystemLogFiltersPayload, 'page' | 'pageSize'>;
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
  logsReportFilters: {
    startDate: undefined,
    endDate: undefined,
    userId: undefined,
    action: '',
  },
};
