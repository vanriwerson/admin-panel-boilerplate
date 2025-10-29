import { useEffect, useState, useMemo } from 'react';
import {
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  OutlinedInput,
  Checkbox,
  ListItemText,
  type SelectChangeEvent,
  CircularProgress,
  Box,
} from '@mui/material';
import { useSystemResources } from '../../hooks';
import type { SystemResource } from '../../interfaces';

interface Props {
  value: number[];
  onChange: (value: number[]) => void;
}

export default function SystemResourceSelect({ value, onChange }: Props) {
  const { fetchSystemResourcesForSelect, loading } = useSystemResources();
  const [options, setOptions] = useState<SystemResource[]>([]);

  useEffect(() => {
    fetchSystemResourcesForSelect()
      .then((data) => setOptions(data))
      .catch((err) =>
        console.error('Erro ao carregar opções de permissões:', err)
      );
  }, [fetchSystemResourcesForSelect]);

  function handleChange(event: SelectChangeEvent<string[]>) {
    const { value } = event.target;
    const newValue =
      typeof value === 'string'
        ? value.split(',').map(Number)
        : value.map(Number);
    onChange(newValue);
  }

  const selectedNames = useMemo(() => {
    if (!options.length) return [];
    return options
      .filter((r) => value.includes(r.id!))
      .map((r) => r.exhibitionName);
  }, [options, value]);

  if (loading && !options.length) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" p={2}>
        <CircularProgress size={24} />
      </Box>
    );
  }

  return (
    <FormControl fullWidth>
      <InputLabel>Permissões</InputLabel>
      <Select<string[]>
        multiple
        value={value.map(String)}
        onChange={handleChange}
        input={<OutlinedInput label="Permissões" />}
        renderValue={() => selectedNames.join(', ')}
      >
        {options.map((resource) => (
          <MenuItem key={resource.id} value={String(resource.id)}>
            <Checkbox checked={value.includes(resource.id!)} />
            <ListItemText primary={resource.exhibitionName} />
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
