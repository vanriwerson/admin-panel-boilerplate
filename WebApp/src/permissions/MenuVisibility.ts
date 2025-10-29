import type { AuthUser, MenuItem } from '../interfaces';
import { hasPermission, isRootUser } from './Rules';
import { menuItems as items } from '../helpers/menuItems';

export function filterMenuByPermissions(authUser: AuthUser | null): MenuItem[] {
  return items.filter((item) => {
    if (!item.permission) return true;
    if (!authUser) return false;
    if (isRootUser(authUser)) return true;
    return hasPermission(authUser, item.permission);
  });
}
