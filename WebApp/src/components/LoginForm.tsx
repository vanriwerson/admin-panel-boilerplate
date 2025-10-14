import { useState } from 'react';
import { TextField, Button, Box, Alert } from '@mui/material';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import type { LoginPayload } from '../types';
import api from '../api';

const LoginForm = () => {
  const [form, setForm] = useState<LoginPayload>({
    identifier: '',
    password: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const response = await api.post('/auth/login', form);

      const { token } = response.data;
      localStorage.setItem('token', token);

      navigate('/dashboard');
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.error) {
        setError(err.response.data.error);
      } else {
        setError('Erro ao tentar logar. Tente novamente.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
    >
      {error && <Alert severity="error">{error}</Alert>}
      <TextField
        label="Username ou Email"
        name="identifier"
        value={form.identifier}
        onChange={handleChange}
        required
      />
      <TextField
        label="Senha"
        type="password"
        name="password"
        value={form.password}
        onChange={handleChange}
        required
      />
      <Button
        type="submit"
        variant="contained"
        color="primary"
        disabled={loading}
      >
        {loading ? 'Entrando...' : 'Entrar'}
      </Button>
    </Box>
  );
};

export default LoginForm;
