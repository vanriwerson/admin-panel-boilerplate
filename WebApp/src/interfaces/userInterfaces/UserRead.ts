import type { SystemResource } from '../SystemResource';

export interface UserRead {
  id: number;
  username: string;
  email: string;
  fullName: string;
  permissions?: SystemResource[];
  createdAt?: Date;
  updatedAt?: Date;
}
