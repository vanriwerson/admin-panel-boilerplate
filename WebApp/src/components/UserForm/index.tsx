import { useState, useEffect } from 'react';
import { Box, TextField, Button, FormHelperText } from '@mui/material';
import type { UserFormValues, UserRead } from '../../interfaces';
import { mapSystemResourcesToFormValue } from '../../helpers';
import { useAuth } from '../../hooks';
import SystemResourceSelect from '../SystemResourcesSelect';
import { canEditPassword, canEditPermissions } from '../../permissions/Rules';

interface Props {
  onSubmit: (user: UserFormValues) => void;
  user?: UserRead;
}

export default function UserForm({ onSubmit, user }: Props) {
  const [form, setForm] = useState<UserFormValues>({
    username: '',
    email: '',
    password: '',
    fullName: '',
    permissions: [],
  });

  const [error, setError] = useState('');
  const { authUser } = useAuth();

  const showPasswordField = authUser && canEditPassword(authUser, user);
  const canEditPerms = authUser && canEditPermissions(authUser, user);

  useEffect(() => {
    if (user) {
      setForm({
        id: user.id,
        username: user.username,
        email: user.email,
        password: '',
        fullName: user.fullName,
        permissions: mapSystemResourcesToFormValue(user.permissions),
      });
    }
  }, [user]);

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  function handlePermissionsChange(permissions: number[]) {
    setForm({ ...form, permissions });
    if (permissions.length > 0) setError('');
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (form.permissions.length === 0) {
      setError('É necessário conceder ao menos uma permissão.');
      return;
    }
    setError('');
    onSubmit(form);
  }

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{
        alignItems: 'center',
        display: 'flex',
        flexWrap: 'wrap',
        gap: 2,
        justifyContent: 'center',
        marginBottom: 4,
        maxWidth: 800,
      }}
    >
      <TextField
        label="Nome Completo"
        name="fullName"
        value={form.fullName}
        onChange={handleChange}
        required
        fullWidth
      />

      <TextField
        label="Usuário"
        name="username"
        value={form.username}
        onChange={handleChange}
        required
        sx={{ flexGrow: 1 }}
      />

      <TextField
        label="E-mail"
        name="email"
        value={form.email}
        onChange={handleChange}
        required
        sx={{ flexGrow: 1 }}
      />

      {showPasswordField && (
        <TextField
          label="Senha"
          name="password"
          type="password"
          value={form.password}
          onChange={handleChange}
          required={!user}
          sx={{ flexGrow: 1 }}
        />
      )}

      <Box sx={{ width: '100%' }}>
        <SystemResourceSelect
          value={form.permissions}
          onChange={handlePermissionsChange}
          readOnly={!canEditPerms}
        />
        {error && <FormHelperText error>{error}</FormHelperText>}
      </Box>

      <Button variant="contained" type="submit">
        {user ? 'Atualizar' : 'Cadastrar'}
      </Button>
    </Box>
  );
}
