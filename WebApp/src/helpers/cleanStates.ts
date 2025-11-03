import type { SystemResource, UserFormValues } from '../interfaces';

interface FormStates {
  userForm: UserFormValues;
  systemResource: SystemResource;
}

export const cleanStates: FormStates = {
  userForm: {
    username: '',
    email: '',
    fullName: '',
    password: '',
    permissions: [],
  },
  systemResource: {
    name: '',
    exhibitionName: '',
  },
};
