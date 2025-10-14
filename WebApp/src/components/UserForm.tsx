import { useState, useEffect } from 'react';
import { Box, TextField, Button } from '@mui/material';
import type { UserCreateDto, UserReadDto } from '../types';

interface Props {
  onSubmit: (user: UserCreateDto) => void;
  user?: UserReadDto;
}

export default function UserForm({ onSubmit, user }: Props) {
  const [form, setForm] = useState<UserCreateDto>({
    username: '',
    email: '',
    password: '',
    fullName: '',
  });

  useEffect(() => {
    if (user) {
      setForm({
        username: user.username,
        email: user.email,
        password: '',
        fullName: user.fullName,
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
      sx={{ display: 'flex', flexDirection: 'column', gap: 2, width: 400 }}
    >
      <TextField
        label="Nome Completo"
        name="fullName"
        value={form.fullName}
        onChange={handleChange}
        required
      />
      <TextField
        label="Usuário"
        name="username"
        value={form.username}
        onChange={handleChange}
        required
      />
      <TextField
        label="E-mail"
        name="email"
        value={form.email}
        onChange={handleChange}
        required
      />
      {!user && (
        <TextField
          label="Senha"
          name="password"
          type="password"
          value={form.password}
          onChange={handleChange}
          required
        />
      )}
      <Button variant="contained" type="submit">
        {user ? 'Atualizar' : 'Cadastrar'}
      </Button>
    </Box>
  );
}
