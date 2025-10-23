import type { SystemResource } from '../SystemResource';

export interface AuthUser {
  username: string;
  fullName: string;
  permissions: SystemResource[];
}
