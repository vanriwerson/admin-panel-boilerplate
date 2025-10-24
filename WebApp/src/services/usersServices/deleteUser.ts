import api from '../../api';
import type { UserRead } from '../../interfaces';

export async function deleteUser(user: UserRead) {
  await api.delete(`/users/${user.id}`);
}
