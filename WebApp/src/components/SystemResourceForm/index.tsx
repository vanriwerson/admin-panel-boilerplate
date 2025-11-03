import { useState, useEffect } from 'react';
import { Box, TextField, Button } from '@mui/material';
import type { SystemResource } from '../../interfaces';
import { cleanStates } from '../../helpers';

interface Props {
  onSubmit: (resource: SystemResource) => void;
  resource?: SystemResource;
}

export default function SystemResourceForm({ onSubmit, resource }: Props) {
  const [form, setForm] = useState(cleanStates.systemResource);

  useEffect(() => {
    if (resource) {
      setForm({
        id: resource.id,
        name: resource.name,
        exhibitionName: resource.exhibitionName,
      });
    }
  }, [resource]);

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    onSubmit(form);
    setForm(cleanStates.systemResource);
  }

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{
        display: 'flex',
        flexDirection: 'column',
        gap: 2,
        mb: 2,
        width: '100%',
        maxWidth: 400,
      }}
    >
      <TextField
        label="Nome"
        name="name"
        value={form.name}
        onChange={handleChange}
        required
        fullWidth
      />
      <TextField
        label="Nome de exibição"
        name="exhibitionName"
        value={form.exhibitionName}
        onChange={handleChange}
        required
        fullWidth
      />
      <Button variant="contained" type="submit">
        {resource ? 'Atualizar' : 'Cadastrar'}
      </Button>
    </Box>
  );
}
