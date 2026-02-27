import type { SystemResourceOption } from '../systemResourcesInterfaces/SystemResourceOption';

export interface UserRead {
  id: number;
  username: string;
  email: string;
  fullName: string;
  permissions: SystemResourceOption[];
}
