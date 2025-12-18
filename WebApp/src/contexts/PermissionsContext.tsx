import { createContext, useState, useEffect, type ReactNode } from 'react';
import { listSystemResourcesForSelect } from '../services';
import type { PermissionsContextProps } from '../interfaces';
import { cleanStates } from '../helpers';

const PermissionsContext = createContext<PermissionsContextProps | undefined>(
  undefined
);
export default PermissionsContext;

export function PermissionsProvider({ children }: { children: ReactNode }) {
  const [permissionsMap, setPermissionsMap] = useState<Record<string, string>>(
    cleanStates.initialPermissionsMap
  );
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadPermissions = async () => {
    try {
      setLoading(true);
      const systemResources = await listSystemResourcesForSelect();

      // Transforma array de SystemResource em objeto mapeado por name
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
  };

  const refreshPermissions = async () => {
    await loadPermissions();
  };

  useEffect(() => {
    loadPermissions();
  }, []);

  const value: PermissionsContextProps = {
    permissionsMap,
    loading,
    error,
    refreshPermissions,
  };

  return (
    <PermissionsContext.Provider value={value}>
      {children}
    </PermissionsContext.Provider>
  );
}

// Mantém compatibilidade com código existente - versão síncrona (deprecated)
export const PermissionsMap = {
  ROOT: 'root',
  USERS: 'users',
  RESOURCES: 'resources',
  REPORTS: 'reports',
  DASHBOARD: 'dashboard',
} as const;

export type PermissionName =
  (typeof PermissionsMap)[keyof typeof PermissionsMap];
