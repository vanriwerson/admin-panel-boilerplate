import { useState } from 'react';
import { Box, Typography, Paper, TextField, Button } from '@mui/material';
import { ActionsSelect, ReportsTable, UsersSelect } from '../../components';

export default function Reports() {
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

      <Paper sx={{ p: 3, mb: 4 }}>
        <Box
          component="form"
          sx={{
            display: 'flex',
            flexWrap: 'wrap',
            gap: 2,
            alignItems: 'flex-end',
          }}
        >
          <Box sx={{ flex: { xs: '1 1 100%', sm: '1 1 48%' } }}>
            <TextField
              label="Data Inicial"
              type="date"
              fullWidth
              slotProps={{ inputLabel: { shrink: true } }}
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
            />
          </Box>

          <Box sx={{ flex: { xs: '1 1 100%', sm: '1 1 48%' } }}>
            <TextField
              label="Data Final"
              type="date"
              fullWidth
              slotProps={{ inputLabel: { shrink: true } }}
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
            />
          </Box>

          <Box sx={{ flex: { xs: '1 1 100%', sm: '1 1 65%' } }}>
            <UsersSelect
              value={selectedUserIds}
              onChange={setSelectedUserIds}
            />
          </Box>

          <Box sx={{ flex: { xs: '1 1 100%', sm: '1 1 30%' } }}>
            <ActionsSelect
              value={selectedAction}
              onChange={(val) => setSelectedAction(val)}
            />
          </Box>

          <Box
            sx={{
              flex: '1 1 100%',
              display: 'flex',
              justifyContent: 'flex-end',
            }}
          >
            <Button variant="contained" onClick={handleResetFilters}>
              Limpar filtros
            </Button>
          </Box>
        </Box>
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
