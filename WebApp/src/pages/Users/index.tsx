import { useState } from 'react';
import { Container, Typography, Modal, Box, Button } from '@mui/material';
import { UserForm, UsersTable } from '../../components';
import type { UserFormValues, UserRead } from '../../interfaces';
import { useUsers } from '../../hooks';

export default function Users() {
  const { fetchUsers, addUser, editUser, removeUser } = useUsers();
  const [editingUser, setEditingUser] = useState<UserRead | null>(null);
  const [open, setOpen] = useState(false);

  async function handleCreate(user: UserFormValues) {
    try {
      await addUser(user);
      alert('✅ Usuário cadastrado com sucesso!');
      fetchUsers(); // atualiza tabela
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao cadastrar usuário');
    }
  }

  async function handleUpdate(user: UserFormValues) {
    if (!editingUser) return;
    try {
      await editUser({ ...editingUser, ...user });
      alert('✅ Usuário atualizado com sucesso!');
      setOpen(false);
      fetchUsers();
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao atualizar usuário');
    }
  }

  async function handleDelete(id: number) {
    const confirmDelete = confirm(
      `Tem certeza que deseja excluir o usuário selecionado?`
    );
    if (!confirmDelete) return;

    try {
      await removeUser(id);
      alert('🗑️ Usuário excluído com sucesso!');
      fetchUsers(); // atualiza tabela
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao excluir usuário');
    }
  }

  function handleEdit(user: UserRead) {
    setEditingUser(user);
    setOpen(true);
  }

  return (
    <Container
      sx={{
        mt: 4,
        alignItems: 'center',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        textAlign: 'center',
      }}
    >
      <Typography variant="h5" gutterBottom>
        Gerenciamento de Usuários
      </Typography>

      <UserForm onSubmit={handleCreate} />

      <UsersTable onEdit={handleEdit} onDelete={handleDelete} />

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

          {editingUser && (
            <UserForm user={editingUser} onSubmit={handleUpdate} />
          )}

          <Box display="flex" justifyContent="flex-end" mt={2}>
            <Button onClick={() => setOpen(false)}>Fechar</Button>
          </Box>
        </Box>
      </Modal>
    </Container>
  );
}
