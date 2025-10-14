import { useEffect, useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
} from '@mui/material';
import { Edit, Delete } from '@mui/icons-material';
import api from '../api';
import type { UserReadDto } from '../types';

interface Props {
  onEdit: (user: UserReadDto) => void;
}

export default function UserTable({ onEdit }: Props) {
  const [users, setUsers] = useState<UserReadDto[]>([]);

  async function fetchUsers() {
    const { data } = await api.get('/users');
    setUsers(data);
  }

  async function handleDelete(id: number) {
    if (!confirm('Deseja excluir este usuário?')) return;
    await api.delete(`/users/${id}`);
    fetchUsers();
  }

  useEffect(() => {
    fetchUsers();
  }, []);

  return (
    <TableContainer component={Paper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Usuário</TableCell>
            <TableCell>Nome Completo</TableCell>
            <TableCell>Email</TableCell>
            <TableCell>Criado em</TableCell>
            <TableCell>Ações</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map((u) => (
            <TableRow key={u.id}>
              <TableCell>{u.username}</TableCell>
              <TableCell>{u.fullName}</TableCell>
              <TableCell>{u.email}</TableCell>
              <TableCell>
                {new Date(u.createdAt).toLocaleDateString('pt-BR')}
              </TableCell>
              <TableCell>
                <IconButton onClick={() => onEdit(u)}>
                  <Edit />
                </IconButton>
                <IconButton onClick={() => handleDelete(u.id)}>
                  <Delete />
                </IconButton>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
