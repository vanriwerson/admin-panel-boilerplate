import { useEffect, useMemo, useState } from 'react';
import {
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  OutlinedInput,
  Checkbox,
  ListItemText,
  CircularProgress,
  Box,
  TextField,
} from '@mui/material';
import { useUsers } from '../../hooks';
import type { SelectChangeEvent } from '@mui/material';
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
  const [searchKey, setSearchKey] = useState('');

  useEffect(() => {
    async function loadOptions() {
      const data = await fetchUsersForSelect();
      setOptions(data);
    }
    loadOptions();
  }, [fetchUsersForSelect]);

  const filteredOptions = useMemo(() => {
    if (!searchKey) return options;
    return options.filter((u) =>
      u.fullName.toLowerCase().includes(searchKey.toLowerCase())
    );
  }, [options, searchKey]);

  const selectedNames = useMemo(() => {
    if (!options.length) return [];
    return options.filter((u) => value.includes(u.id!)).map((u) => u.fullName);
  }, [options, value]);

  function handleChange(event: SelectChangeEvent<string[]>) {
    const { value } = event.target;
    const newValue =
      typeof value === 'string'
        ? value.split(',').map(Number)
        : value.map(Number);
    onChange(newValue);
  }

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
        label="Usuários"
        value={selectedNames.join(', ') || 'Nenhum usuário selecionado'}
        slotProps={{ input: { readOnly: true } }}
        fullWidth
      />
    );
  }

  return (
    <FormControl fullWidth>
      <InputLabel>Usuários</InputLabel>
      <Select<string[]>
        value={value.map(String)}
        onChange={handleChange}
        input={<OutlinedInput label="Usuários" />}
        renderValue={() => selectedNames.join(', ')}
        MenuProps={{ PaperProps: { style: { maxHeight: 300 } } }}
      >
        <Box px={2} py={1}>
          <TextField
            size="small"
            placeholder="Buscar usuário..."
            fullWidth
            value={searchKey}
            onChange={(e) => setSearchKey(e.target.value)}
          />
        </Box>

        {filteredOptions.map((user) => (
          <MenuItem key={user.id} value={String(user.id)}>
            <Checkbox checked={value.includes(user.id!)} />
            <ListItemText primary={user.fullName} />
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
