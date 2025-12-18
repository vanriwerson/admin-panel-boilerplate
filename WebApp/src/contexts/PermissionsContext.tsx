import {
  createContext,
  useState,
  useEffect,
  type ReactNode,
  useCallback,
  useMemo,
} from 'react';
import type {
  AuthUser,
  MenuItem,
  PermissionsContextProps,
} from '../interfaces';
import {
  cleanStates,
  getPageTitleIcons,
  menuItems as baseMenuItems,
} from '../helpers';
import { useSystemResources } from '../hooks';
import { hasPermission, isRootUser } from '../permissions/Rules';

const PermissionsContext = createContext<PermissionsContextProps | undefined>(
  undefined
);
export default PermissionsContext;

export function PermissionsProvider({ children }: { children: ReactNode }) {
  const { fetchSystemResourcesForSelect } = useSystemResources();
  const [permissionsMap, setPermissionsMap] = useState<Record<string, string>>(
    cleanStates.initialPermissionsMap
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadPermissions = useCallback(async () => {
    try {
      setLoading(true);
      const systemResources = await fetchSystemResourcesForSelect();

      const map = systemResources.reduce((acc, resource) => {
        acc[resource.name.toUpperCase()] = resource.name;
        return acc;
      }, {} as Record<string, string>);

      setPermissionsMap(map);
      setError(null);
    } catch (err) {
      console.error('Erro ao carregar permissions:', err);
      setError('Erro ao carregar permissões');
    } finally {
      setLoading(false);
    }
  }, [fetchSystemResourcesForSelect]);

  const refreshPermissions = async () => {
    await loadPermissions();
  };

  const menuItems: MenuItem[] = useMemo(() => {
    return baseMenuItems.map((item) => {
      if (!item.permission) return item;
      return {
        ...item,
        permission:
          permissionsMap[item.permission.toUpperCase()] || item.permission,
      };
    });
  }, [permissionsMap]);

  const pageTitleIcons = getPageTitleIcons(menuItems);

  const getMenuItemsForUser = useCallback(
    (authUser: AuthUser | null): MenuItem[] => {
      return menuItems.filter((item) => {
        if (!item.permission) return true;
        if (!authUser) return false;
        if (isRootUser(authUser)) return true;
        return hasPermission(authUser, item.permission);
      });
    },
    [menuItems]
  );

  useEffect(() => {
    loadPermissions();
  }, [loadPermissions]);

  return (
    <PermissionsContext.Provider
      value={{
        permissionsMap,
        pageTitleIcons,
        menuItems,
        loading,
        error,
        refreshPermissions,
        getMenuItemsForUser,
      }}
    >
      {children}
    </PermissionsContext.Provider>
  );
}
