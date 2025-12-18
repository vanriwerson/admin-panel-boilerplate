import { useState, useEffect } from 'react';
import { Box, TextField, Button, FormHelperText, Paper } from '@mui/material';
import type { UserFormValues, UserRead } from '../../interfaces';
import { cleanStates, mapSystemResourcesToFormValue } from '../../helpers';
import { useAuth } from '../../hooks';
import SystemResourceSelect from '../SystemResourcesSelect';
import { canEditPassword, canEditPermissions } from '../../permissions/Rules';

interface Props {
  onSubmit: (user: UserFormValues) => void;
  user?: UserRead;
}

export default function UserForm({ onSubmit, user }: Props) {
  const [form, setForm] = useState(cleanStates.userForm);

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

    if (user && (!form.password || form.password.trim() === '')) {
      delete form.password;
    }
    onSubmit(form);
    setForm(cleanStates.userForm);
  }

  return (
    <Paper
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
        padding: 2,
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

      <Box display="flex" width="100%" gap={2} justifyContent="center">
        <Button variant="contained" type="submit">
          {user ? 'Atualizar' : 'Cadastrar'}
        </Button>

        <Button
          variant="contained"
          color="secondary"
          onClick={() => setForm(cleanStates.userForm)}
        >
          Limpar
        </Button>
      </Box>
    </Paper>
  );
}
