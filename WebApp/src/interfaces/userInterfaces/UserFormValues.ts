export interface UserFormValues {
  id?: number;
  username: string;
  email: string;
  fullName: string;
  password?: string;
  permissions: number[];
}
