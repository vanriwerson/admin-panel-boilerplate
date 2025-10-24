import { useCallback, useState } from 'react';
import type { SystemResource, SystemResourcesPagination } from '../interfaces';
import {
  createSystemResource,
  updateSystemResource,
  deleteSystemResource,
  listSystemResources,
  listSystemResourcesForSelect,
} from '../services';

export function useSystemResources() {
  const [resources, setResources] = useState<SystemResource[]>([]);
  const [pagination, setPagination] = useState<SystemResourcesPagination>({
    totalItems: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
    data: [],
  });
  const [loading, setLoading] = useState(false);

  const fetchSystemResources = useCallback(
    async (
      pageNumber = pagination.page,
      pageSize = pagination.pageSize,
      searchKey = ''
    ) => {
      try {
        setLoading(true);
        const data = await listSystemResources(pageNumber, pageSize, searchKey);
        setResources(data.data);
        setPagination(data);
      } catch (error) {
        console.error('Erro ao buscar recursos do sistema:', error);
      } finally {
        setLoading(false);
      }
    },
    [pagination.page, pagination.pageSize]
  );

  const create = useCallback(
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

  const update = useCallback(
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

  const remove = useCallback(
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

  const fetchOptions = useCallback(async () => {
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
    setPagination,
    fetchSystemResources,
    create,
    update,
    remove,
    fetchOptions,
  };
}
