import type { UserRead } from '../userInterfaces/UserRead';

export interface SystemLog {
  id: number;
  userId: number;
  action: string;
  createdAt: Date;
  user: UserRead;
}
