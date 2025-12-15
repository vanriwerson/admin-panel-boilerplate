export const PermissionsMap = {
  ROOT: 'root',
  USERS: 'users',
  RESOURCES: 'resources',
  REPORTS: 'reports',
  DASHBOARD: 'dashboard',
} as const;

export type PermissionName =
  (typeof PermissionsMap)[keyof typeof PermissionsMap];
