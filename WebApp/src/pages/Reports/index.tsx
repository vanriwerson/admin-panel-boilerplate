import { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  MenuItem,
  TextField,
  Button,
} from '@mui/material';
import Grid from '@mui/material/Grid';
import UsersSelect from '../../components/UsersSelect';
import ReportsTable from '../../components/ReportsTable';
// import { useReports } from '../../hooks';

export default function Reports() {
  // const { fetchReports } = useReports();

  const [startDate, setStartDate] = useState<string>('');
  const [endDate, setEndDate] = useState<string>('');
  const [selectedUserIds, setSelectedUserIds] = useState<number[]>([]);
  const [selectedAction, setSelectedAction] = useState<string>('');

  const handleResetFilters = () => {
    setStartDate('');
    setEndDate('');
    setSelectedUserIds([]);
    setSelectedAction('');
  };

  return (
    <Box p={3}>
      <Typography variant="h4" gutterBottom>
        Relatórios de Logs
      </Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <TextField
            label="Data Inicial"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
          <TextField
            label="Data Final"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
          <UsersSelect value={selectedUserIds} onChange={setSelectedUserIds} />

          <TextField
            label="Ação"
            select
            fullWidth
            value={selectedAction}
            onChange={(e) => setSelectedAction(e.target.value)}
          >
            {['create', 'update', 'delete', 'login', 'senha'].map((action) => (
              <MenuItem key={action} value={action}>
                {action}
              </MenuItem>
            ))}
          </TextField>

          <Button variant="outlined" onClick={handleResetFilters}>
            Limpar filtros
          </Button>
        </Grid>
      </Paper>

      <ReportsTable
        filters={{
          startDate: startDate || undefined,
          endDate: endDate || undefined,
          userId: selectedUserIds[0] || undefined,
          action: selectedAction || undefined,
        }}
      />
    </Box>
  );
}
