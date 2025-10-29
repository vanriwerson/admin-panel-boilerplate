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
import { useSystemResources, useAuth } from '../../hooks';
import { filterAssignablePermissions } from '../../permissions/rules';
import type { SystemResource } from '../../interfaces';
import type { SelectChangeEvent } from '@mui/material';

interface Props {
  value: number[];
  onChange: (value: number[]) => void;
  readOnly?: boolean;
}

export default function SystemResourceSelect({
  value,
  onChange,
  readOnly = false,
}: Props) {
  const { authUser } = useAuth();
  const { fetchSystemResourcesForSelect, loading } = useSystemResources();

  const [options, setOptions] = useState<SystemResource[]>([]);

  useEffect(() => {
    async function loadOptions() {
      const data = await fetchSystemResourcesForSelect();
      setOptions(data);
    }
    loadOptions();
  }, [fetchSystemResourcesForSelect]);

  const filteredOptions = useMemo(() => {
    if (!authUser) return [];
    return filterAssignablePermissions(authUser, options);
  }, [authUser, options]);

  const selectedNames = useMemo(() => {
    if (!options.length) return [];
    return options
      .filter((r) => value.includes(r.id!))
      .map((r) => r.exhibitionName);
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
        label="Permissões"
        value={selectedNames.join(', ') || 'Sem permissões'}
        slotProps={{ input: { readOnly: true } }}
        fullWidth
      />
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
        {filteredOptions.map((resource) => (
          <MenuItem key={resource.id} value={String(resource.id)}>
            <Checkbox checked={value.includes(resource.id!)} />
            <ListItemText primary={resource.exhibitionName} />
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
