import React, { useEffect, useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TableContainer,
  Paper,
  TablePagination,
  IconButton,
  Typography,
  Box,
  TextField,
} from '@mui/material';
import { Edit, Delete, Search } from '@mui/icons-material';

import type { UserRead } from '../../interfaces';
import { useUsers } from '../../hooks';

interface UsersTableProps {
  onEdit: (user: UserRead) => void;
  onDelete?: (id: number) => void;
}

export default function UsersTable({ onEdit, onDelete }: UsersTableProps) {
  const { users, pagination, loading, fetchUsers, setPagination } = useUsers();
  const [searchKey, setSearchKey] = useState('');

  useEffect(() => {
    fetchUsers(pagination.page, pagination.pageSize, searchKey);
  }, [fetchUsers, pagination.page, pagination.pageSize, searchKey]);

  const handleChangePage = (_: unknown, newPage: number) => {
    setPagination((prev) => ({ ...prev, page: newPage + 1 }));
  };

  const handleChangeRowsPerPage = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPagination({
      ...pagination,
      page: 1,
      pageSize: parseInt(e.target.value, 10),
    });
  };

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPagination((prev) => ({ ...prev, page: 1 }));
    fetchUsers(1, pagination.pageSize, searchKey);
  };

  return (
    <Paper sx={{ p: 2 }}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h6">Usuários cadastrados</Typography>

        <form onSubmit={handleSearchSubmit}>
          <TextField
            size="small"
            variant="outlined"
            placeholder="Buscar usuário..."
            value={searchKey}
            onChange={(e) => setSearchKey(e.target.value)}
            slotProps={{
              input: {
                startAdornment: <Search />,
              },
            }}
          />
        </form>
      </Box>

      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Usuário</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Nome completo</TableCell>
              <TableCell>Criado em</TableCell>
              <TableCell>Atualizado em</TableCell>
              <TableCell align="right">Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  Carregando...
                </TableCell>
              </TableRow>
            ) : users.length > 0 ? (
              users.map((user: UserRead) => (
                <TableRow key={user.id} hover>
                  <TableCell>{user.id}</TableCell>
                  <TableCell>{user.username}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.fullName}</TableCell>
                  <TableCell>
                    {new Date(user.createdAt).toLocaleString()}
                  </TableCell>
                  <TableCell>
                    {new Date(user.updatedAt).toLocaleString()}
                  </TableCell>
                  <TableCell align="right">
                    <IconButton color="primary" onClick={() => onEdit(user)}>
                      <Edit />
                    </IconButton>
                    <IconButton
                      color="error"
                      onClick={() => onDelete?.(user.id)}
                    >
                      <Delete />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  Nenhum usuário encontrado
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Box display="flex" justifyContent="flex-end">
        <TablePagination
          component="div"
          count={pagination.totalItems}
          page={pagination.page - 1}
          onPageChange={handleChangePage}
          rowsPerPage={pagination.pageSize}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Itens por página:"
          rowsPerPageOptions={[5, 10, 25]}
        />
      </Box>
    </Paper>
  );
}
