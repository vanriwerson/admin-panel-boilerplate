import type { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import type { MenuItem } from '../MenuItem';
import type { AuthUser } from '../userInterfaces/AuthUser';

export interface PermissionsContextProps {
  permissionsMap: Record<string, string>;
  pageTitleIcons: Record<string, IconDefinition>;
  menuItems: MenuItem[];
  loading: boolean;
  error: string | null;
  refreshPermissions: () => Promise<void>;
  getMenuItemsForUser: (authUser: AuthUser | null) => MenuItem[];
}
