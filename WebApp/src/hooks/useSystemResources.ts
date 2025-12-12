import { useCallback, useState } from 'react';
import type { SystemResource } from '../interfaces';
import {
  createSystemResource,
  updateSystemResource,
  deleteSystemResource,
  listSystemResources,
  listSystemResourcesForSelect,
} from '../services';
import { cleanStates } from '../helpers';

export function useSystemResources() {
  const [resources, setResources] = useState<SystemResource[]>([]);
  const [pagination, setPagination] = useState(cleanStates.tablePagination);
  const [loading, setLoading] = useState(false);

  const fetchSystemResources = useCallback(
    async (
      pageNumber = pagination.page,
      pageSize = pagination.pageSize,
      searchKey = ''
    ) => {
      try {
        setLoading(true);
        const response = await listSystemResources(
          pageNumber,
          pageSize,
          searchKey
        );
        setResources(response.data);
        setPagination({
          totalItems: response.totalItems,
          page: response.page,
          pageSize: response.pageSize,
          totalPages: response.totalPages,
        });
      } catch (error) {
        console.error('Erro ao buscar recursos do sistema:', error);
      } finally {
        setLoading(false);
      }
    },
    [pagination.page, pagination.pageSize]
  );

  const addSystemResource = useCallback(
    async (resource: SystemResource) => {
      try {
        await createSystemResource(resource);
        await fetchSystemResources();
      } catch (error) {
        console.error('Erro ao criar recurso do sistema:', error);
      }
    },
    [fetchSystemResources]
  );

  const editSystemResource = useCallback(
    async (resource: SystemResource) => {
      try {
        await updateSystemResource(resource);
        await fetchSystemResources();
      } catch (error) {
        console.error('Erro ao atualizar recurso do sistema:', error);
      }
    },
    [fetchSystemResources]
  );

  const removeSystemResource = useCallback(
    async (id: string) => {
      try {
        await deleteSystemResource(id);
        await fetchSystemResources();
      } catch (error) {
        console.error('Erro ao excluir recurso do sistema:', error);
      }
    },
    [fetchSystemResources]
  );

  const fetchSystemResourcesForSelect = useCallback(async () => {
    try {
      const data = await listSystemResourcesForSelect();
      return data;
    } catch (error) {
      console.error('Erro ao listar opções de recursos do sistema:', error);
      return [];
    }
  }, []);

  return {
    resources,
    pagination,
    loading,
    fetchSystemResources,
    addSystemResource,
    editSystemResource,
    removeSystemResource,
    fetchSystemResourcesForSelect,
    setPagination,
  };
}
