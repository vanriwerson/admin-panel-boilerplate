import { useState } from 'react';
import { Container, Typography, Modal, Box } from '@mui/material';
import type { UserCreateDto, UserReadDto } from '../types';
import api from '../api';
import { UserForm, UserTable } from '../components';

export default function Dashboard() {
  const [editingUser, setEditingUser] = useState<UserReadDto | null>(null);
  const [open, setOpen] = useState(false);

  async function handleCreate(user: UserCreateDto) {
    try {
      await api.post('/users', user);
      alert('Usuário cadastrado com sucesso!');
      window.location.reload();
    } catch (err) {
      console.error(err);
      alert('Erro ao cadastrar usuário');
    }
  }

  async function handleUpdate(user: UserCreateDto) {
    try {
      await api.put(`/users/${editingUser?.id}`, user);
      alert('Usuário atualizado com sucesso!');
      setOpen(false);
      window.location.reload();
    } catch (err) {
      console.error(err);
      alert('Erro ao atualizar usuário');
    }
  }

  function handleEdit(user: UserReadDto) {
    setEditingUser(user);
    setOpen(true);
  }

  return (
    <Container sx={{ mt: 4 }}>
      <Typography variant="h5" gutterBottom>
        Cadastro de Usuários
      </Typography>

      <UserForm onSubmit={handleCreate} />
      <UserTable onEdit={handleEdit} />

      <Modal open={open} onClose={() => setOpen(false)}>
        <Box
          sx={{
            bgcolor: 'background.paper',
            p: 4,
            borderRadius: 2,
            m: 'auto',
            mt: '10%',
            width: 400,
          }}
        >
          <Typography variant="h6" gutterBottom>
            Editar Usuário
          </Typography>
          <UserForm user={editingUser!} onSubmit={handleUpdate} />
        </Box>
      </Modal>
    </Container>
  );
}
