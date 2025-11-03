import { useEffect, useState } from 'react';
import { Container, Box, CircularProgress, Alert } from '@mui/material';
import { PageTitle, UserForm } from '../../components';
import { useAuth, useUsers } from '../../hooks';
import type { UserFormValues, UserRead } from '../../interfaces';

export default function Profile() {
  const { authUser } = useAuth();
  const { editUser, fetchUserById, loading, error } = useUsers();

  const [userData, setUserData] = useState<UserRead | null>(null);

  useEffect(() => {
    if (authUser?.id) {
      fetchUserById(authUser.id)
        .then((user) => setUserData(user))
        .catch((err) => console.error('Erro ao buscar usuário logado:', err));
    }
  }, [authUser, fetchUserById]);

  async function handleSubmit(formData: UserFormValues) {
    if (!authUser?.id) return;
    try {
      await editUser({ ...formData, id: authUser.id });
      alert('✅ Perfil atualizado com sucesso!');
    } catch (err) {
      console.error(err);
      alert('❌ Erro ao atualizar perfil');
    }
  }

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mt: 4 }}>
        Ocorreu um erro ao carregar os dados do perfil.
      </Alert>
    );
  }

  if (!userData) return null;

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
      <PageTitle icon="profile" title="Meu Perfil" />

      <UserForm user={userData} onSubmit={handleSubmit} />
    </Container>
  );
}
