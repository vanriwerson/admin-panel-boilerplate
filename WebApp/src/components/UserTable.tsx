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
} from '@mui/material';
import { Edit, Delete } from '@mui/icons-material';
import api from '../api';
import type { UserReadDto, UsersPagination } from '../types';

interface UserTableProps {
  onEdit: (user: UserReadDto) => void;
  onDelete?: (user: UserReadDto) => void;
}

const UserTable: React.FC<UserTableProps> = ({ onEdit, onDelete }) => {
  const [users, setUsers] = useState<UserReadDto[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchUsers = async (pageNumber = 1, pageSize = 10) => {
    try {
      setLoading(true);
      const response = await api.get<UsersPagination>(
        `/users?page=${pageNumber}&pageSize=${pageSize}`
      );
      setUsers(response.data.data);
      setTotalItems(response.data.totalItems);
    } catch (error) {
      console.error('Erro ao buscar usuários:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers(page + 1, rowsPerPage);
  }, [page, rowsPerPage]);

  const handleChangePage = (_: unknown, newPage: number) => setPage(newPage);
  const handleChangeRowsPerPage = (e: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(e.target.value, 10));
    setPage(0);
  };

  return (
    <Paper sx={{ p: 2 }}>
      <Typography variant="h6" gutterBottom>
        Usuários cadastrados
      </Typography>

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
              users.map((user) => (
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
                    <IconButton color="error" onClick={() => onDelete?.(user)}>
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
          count={totalItems}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Itens por página:"
          rowsPerPageOptions={[5, 10, 25]}
        />
      </Box>
    </Paper>
  );
};

export default UserTable;
