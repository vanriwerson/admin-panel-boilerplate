import { useState, useEffect, useCallback } from 'react';
import type {
  SystemLog,
  SystemLogFiltersPayload,
  SystemLogsPagination,
} from '../interfaces';
import { getLogReports } from '../services/systemLogsServices';
import { getErrorMessage } from '../helpers';

export function useReports() {
  const [logs, setLogs] = useState<SystemLog[]>([]);
  const [pagination, setPagination] = useState({
    totalItems: 0,
    page: 1,
    pageSize: 10,
    totalPages: 1,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState<SystemLogFiltersPayload>({});

  const fetchReports = useCallback(
    async (
      overrides?: Partial<SystemLogFiltersPayload> & {
        page?: number;
        pageSize?: number;
      }
    ) => {
      setLoading(true);
      setError(null);

      try {
        const payload: SystemLogFiltersPayload = {
          ...filters,
          page: pagination.page,
          pageSize: pagination.pageSize,
          ...overrides,
        };

        const data: SystemLogsPagination = await getLogReports(payload);

        setLogs(data.data);
        setPagination({
          totalItems: data.totalItems,
          page: data.page,
          pageSize: data.pageSize,
          totalPages: data.totalPages,
        });
      } catch (err) {
        setError(getErrorMessage(err));
        console.error('Erro ao listar logs:', err);
      } finally {
        setLoading(false);
      }
    },
    [filters, pagination.page, pagination.pageSize]
  );

  const setReportFilters = useCallback(
    (newFilters: SystemLogFiltersPayload) => {
      setFilters(newFilters);
      setPagination((prev) => ({ ...prev, page: 1 }));
    },
    []
  );

  useEffect(() => {
    fetchReports();
  }, [fetchReports]);

  return {
    logs,
    pagination,
    loading,
    error,
    filters,
    fetchReports,
    setReportFilters,
    setPagination,
  };
}
