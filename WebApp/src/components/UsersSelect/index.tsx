import { useEffect, useMemo, useState } from 'react';
import { Autocomplete, CircularProgress, TextField, Box } from '@mui/material';
import { useUsers } from '../../hooks';
import type { UserOption } from '../../interfaces';

interface Props {
  value: number[];
  onChange: (value: number[]) => void;
  readOnly?: boolean;
}

export default function UsersSelect({
  value,
  onChange,
  readOnly = false,
}: Props) {
  const { fetchUsersForSelect, loading } = useUsers();
  const [options, setOptions] = useState<UserOption[]>([]);

  useEffect(() => {
    async function loadOptions() {
      const data = await fetchUsersForSelect();
      setOptions(data);
    }
    loadOptions();
  }, [fetchUsersForSelect]);

  const selectedUser = useMemo(
    () => options.find((u) => u.id === value[0]),
    [options, value]
  );

  if (loading && !options.length) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" p={2}>
        <CircularProgress size={24} />
      </Box>
    );
  }

  if (readOnly) {
    return (
      <TextField
        label="Usuário"
        value={selectedUser?.fullName || 'Nenhum usuário selecionado'}
        slotProps={{ inputLabel: { shrink: true } }}
        fullWidth
      />
    );
  }

  return (
    <Autocomplete
      options={options}
      value={selectedUser || null}
      getOptionLabel={(option) => option.fullName}
      isOptionEqualToValue={(opt, val) => opt.id === val.id}
      onChange={(_, newValue) => onChange(newValue ? [newValue.id!] : [])}
      renderInput={(params) => (
        <TextField
          {...params}
          label="Usuário"
          placeholder="Buscar usuário..."
        />
      )}
      fullWidth
      loading={loading}
      loadingText="Carregando usuários..."
      noOptionsText="Nenhum usuário encontrado"
      slotProps={{ listbox: { style: { maxHeight: 300, overflow: 'auto' } } }}
    />
  );
}
