import { useState } from 'react';
import { Container } from '@mui/material';
import {
  PageTitle,
  UserEditionModal,
  UserForm,
  UsersTable,
} from '../../components';
import type { UserFormValues, UserRead } from '../../interfaces';
import { useUsers } from '../../hooks';
import { PermissionsMap } from '../../permissions';

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

  function handleOpenEditionModal(user: UserRead) {
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
      <PageTitle
        icon={PermissionsMap.USERS}
        title="Gerenciamento de Usuários"
      />

      <UserForm onSubmit={handleCreate} />

      <UsersTable onEdit={handleOpenEditionModal} onDelete={handleDelete} />

      <UserEditionModal
        open={open}
        user={editingUser}
        onClose={() => setOpen(false)}
        onSubmit={handleUpdate}
      />
    </Container>
  );
}
