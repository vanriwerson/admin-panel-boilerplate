import type {
  PaginatedResponse,
  SystemLogFiltersPayload,
  SystemResource,
  UserFormValues,
} from '../interfaces';

interface FormStates {
  initialPermissionsMap: Record<string, string>;
  logsReportFilters: Omit<SystemLogFiltersPayload, 'page' | 'pageSize'>;
  systemResource: SystemResource;
  tablePagination: Omit<PaginatedResponse<unknown>, 'data'>;
  userForm: UserFormValues;
}

export const cleanStates: FormStates = {
  initialPermissionsMap: {
    ROOT: 'root',
    USERS: 'users',
    RESOURCES: 'resources',
    REPORTS: 'reports',
    DASHBOARD: 'dashboard',
  },
  logsReportFilters: {
    startDate: undefined,
    endDate: undefined,
    userId: undefined,
    action: '',
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
  userForm: {
    username: '',
    email: '',
    fullName: '',
    password: '',
    permissions: [],
  },
};
