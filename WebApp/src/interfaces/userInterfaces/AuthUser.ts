import type { SystemResource } from '../systemResourcesInterfaces/SystemResource';

export interface AuthUser {
  id: number;
  username: string;
  fullName: string;
  permissions: SystemResource[];
}
