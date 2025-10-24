import type { SystemResource } from '../systemResourcesInterfaces/SystemResource';

export interface AuthUser {
  username: string;
  fullName: string;
  permissions: SystemResource[];
}
