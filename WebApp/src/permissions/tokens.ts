export const PERMISSIONS = {
  ROOT: 'root',
  USERS: 'users',
  RESOURCES: 'resources',
  REPORTS: 'reports',
} as const;

export type ValidPermission = (typeof PERMISSIONS)[keyof typeof PERMISSIONS];
