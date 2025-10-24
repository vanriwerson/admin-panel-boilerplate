import { useEffect, useMemo } from 'react';
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

interface Props {
  value: number[];
  onChange: (value: number[]) => void;
}

export default function SystemResourceSelect({ value, onChange }: Props) {
  const { resources, fetchSystemResources, loading } = useSystemResources();

  useEffect(() => {
    fetchSystemResources();
  }, [fetchSystemResources]);

  function handleChange(event: SelectChangeEvent<string[]>) {
    const { value } = event.target;
    const newValue =
      typeof value === 'string'
        ? value.split(',').map(Number)
        : value.map(Number);
    onChange(newValue);
  }

  const selectedNames = useMemo(() => {
    if (!resources.length) return [];
    return resources
      .filter((r) => value.includes(r.id!))
      .map((r) => r.exhibitionName);
  }, [resources, value]);

  if (loading && !resources.length) {
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
        {resources.map((resource) => (
          <MenuItem key={resource.id} value={String(resource.id)}>
            <Checkbox checked={value.includes(resource.id!)} />
            <ListItemText primary={resource.exhibitionName} />
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
