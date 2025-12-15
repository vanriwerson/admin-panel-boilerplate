import {
  createContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';

import type {
  SystemResource,
  SystemResourcesContextProps,
} from '../interfaces';
import {
  listSystemResources,
  createSystemResource,
  updateSystemResource,
  deleteSystemResource,
  listSystemResourcesForSelect,
} from '../services';
import { cleanStates, getErrorMessage } from '../helpers';

const SystemResourcesContext = createContext<
  SystemResourcesContextProps | undefined
>(undefined);
export default SystemResourcesContext;

export function SystemResourcesProvider({ children }: { children: ReactNode }) {
  const [resources, setResources] = useState<SystemResource[]>([]);
  const [pagination, setPagination] = useState(cleanStates.tablePagination);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchSystemResources = useCallback(
    async (
      page = pagination.page,
      pageSize = pagination.pageSize,
      searchKey = ''
    ) => {
      setLoading(true);
      setError(null);

      try {
        const response = await listSystemResources(page, pageSize, searchKey);

        setResources(response.data);
        setPagination({
          totalItems: response.totalItems,
          page: response.page,
          pageSize: response.pageSize,
          totalPages: response.totalPages,
        });
      } catch (err) {
        setError(getErrorMessage(err));
        console.error('Erro ao listar recursos do sistema:', err);
      } finally {
        setLoading(false);
      }
    },
    [pagination.page, pagination.pageSize]
  );

  const addSystemResource = useCallback(
    async (resource: SystemResource) => {
      setLoading(true);
      try {
        await createSystemResource(resource);
        await fetchSystemResources();
      } finally {
        setLoading(false);
      }
    },
    [fetchSystemResources]
  );

  const editSystemResource = useCallback(
    async (resource: SystemResource) => {
      setLoading(true);
      try {
        await updateSystemResource(resource);
        await fetchSystemResources();
      } finally {
        setLoading(false);
      }
    },
    [fetchSystemResources]
  );

  const removeSystemResource = useCallback(
    async (id: string) => {
      setLoading(true);
      try {
        await deleteSystemResource(id);
        await fetchSystemResources();
      } finally {
        setLoading(false);
      }
    },
    [fetchSystemResources]
  );

  const fetchSystemResourcesForSelect = useCallback(async () => {
    try {
      return await listSystemResourcesForSelect();
    } catch (err) {
      console.error('Erro ao buscar recursos do sistema para select:', err);
      return [];
    }
  }, []);

  useEffect(() => {
    fetchSystemResources();
  }, [fetchSystemResources]);

  return (
    <SystemResourcesContext.Provider
      value={{
        resources,
        pagination,
        loading,
        error,

        fetchSystemResources,
        addSystemResource,
        editSystemResource,
        removeSystemResource,
        fetchSystemResourcesForSelect,

        setPagination,
      }}
    >
      {children}
    </SystemResourcesContext.Provider>
  );
}
