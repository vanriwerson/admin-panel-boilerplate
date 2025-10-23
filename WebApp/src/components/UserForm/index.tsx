import { useState, useEffect } from 'react';
import { Box, TextField, Button } from '@mui/material';
import type { UserFormValues, UserRead } from '../../interfaces';
import { mapSystemResourcesToFormValue } from '../../helpers';

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

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
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
      <TextField
        label="Senha"
        name="password"
        type="password"
        value={form.password}
        onChange={handleChange}
        required
        sx={{ flexGrow: 1 }}
      />
      <Button variant="contained" type="submit">
        {user ? 'Atualizar' : 'Cadastrar'}
      </Button>
    </Box>
  );
}
