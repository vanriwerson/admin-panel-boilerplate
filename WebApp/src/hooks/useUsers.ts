import { useState, useEffect, useCallback } from 'react';
import type { UserRead, UserFormValues } from '../interfaces';
import {
  listUsers,
  createUser,
  updateUser,
  deleteUser,
  listUsersForSelect,
  listUserById,
} from '../services';
import { getErrorMessage } from '../helpers';

export function useUsers() {
  const [users, setUsers] = useState<UserRead[]>([]);
  const [pagination, setPagination] = useState({
    totalItems: 0,
    page: 1,
    pageSize: 10,
    totalPages: 1,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchUsers = useCallback(
    async (
      page = pagination.page,
      pageSize = pagination.pageSize,
      searchKey = ''
    ) => {
      setLoading(true);
      setError(null);
      try {
        const data = await listUsers(page, pageSize, searchKey);
        setUsers(data.data);
        setPagination({
          totalItems: data.totalItems,
          page: data.page,
          pageSize: data.pageSize,
          totalPages: data.totalPages,
        });
      } catch (err) {
        setError(getErrorMessage(err));
        console.error('Erro ao listar usuários:', err);
      } finally {
        setLoading(false);
      }
    },
    [pagination.page, pagination.pageSize]
  );

  const addUser = useCallback(
    async (user: UserFormValues) => {
      setLoading(true);
      try {
        await createUser(user);
        await fetchUsers();
      } finally {
        setLoading(false);
      }
    },
    [fetchUsers]
  );

  const editUser = useCallback(
    async (user: UserFormValues) => {
      setLoading(true);
      try {
        await updateUser(user);
        await fetchUsers();
      } finally {
        setLoading(false);
      }
    },
    [fetchUsers]
  );

  const removeUser = useCallback(
    async (id: number) => {
      setLoading(true);
      try {
        await deleteUser(id.toString());
        await fetchUsers();
      } finally {
        setLoading(false);
      }
    },
    [fetchUsers]
  );

  const fetchUsersForSelect = useCallback(async () => {
    try {
      return await listUsersForSelect();
    } catch (err) {
      console.error('Erro ao buscar usuários para select:', err);
      return [];
    }
  }, []);

  const fetchUserById = useCallback(async (id: number) => {
    try {
      return await listUserById(id);
    } catch (err) {
      console.error('Erro ao buscar usuário:', err);
      return null;
    }
  }, []);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  return {
    users,
    pagination,
    loading,
    error,
    fetchUsers,
    addUser,
    editUser,
    removeUser,
    fetchUsersForSelect,
    fetchUserById,
    setPagination,
  };
}
