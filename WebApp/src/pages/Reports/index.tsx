import { useState } from 'react';
import { Box, Paper, Button } from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import {
  ActionsSelect,
  PageTitle,
  ReportsTable,
  UsersSelect,
} from '../../components';
import { ptBR } from 'date-fns/locale';
import { PermissionsMap } from '../../permissions';

export default function Reports() {
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);
  const [selectedUserIds, setSelectedUserIds] = useState<number[]>([]);
  const [selectedAction, setSelectedAction] = useState<string>('');
  const [error, setError] = useState<string>('');

  const today = new Date();

  const handleResetFilters = () => {
    setStartDate(null);
    setEndDate(null);
    setSelectedUserIds([]);
    setSelectedAction('');
    setError('');
  };

  const handleDateChange = (type: 'start' | 'end', value: Date | null) => {
    const newStart = type === 'start' ? value : startDate;
    const newEnd = type === 'end' ? value : endDate;
    setStartDate(newStart);
    setEndDate(newEnd);
  };

  const startDateString = startDate
    ? startDate.toISOString().split('T')[0]
    : '';
  const endDateString = endDate ? endDate.toISOString().split('T')[0] : '';

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ptBR}>
      <Box p={3}>
        <PageTitle icon={PermissionsMap.REPORTS} title="Relatórios de Logs" />

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
              <DatePicker
                label="Data inicial"
                value={startDate}
                onChange={(date) => handleDateChange('start', date)}
                maxDate={today}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!error,
                  },
                }}
              />
            </Box>

            <Box sx={{ flex: { xs: '1 1 100%', sm: '1 1 48%' } }}>
              <DatePicker
                label="Data final"
                value={endDate}
                onChange={(date) => handleDateChange('end', date)}
                minDate={startDate ?? undefined}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!error,
                  },
                }}
              />
            </Box>

            <Box sx={{ flex: { xs: '1 1 100%', sm: '1 1 65%' } }}>
              <UsersSelect
                value={selectedUserIds}
                onChange={(val: number[]) => setSelectedUserIds([val[0]])}
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
            startDate: startDateString || undefined,
            endDate: endDateString || undefined,
            userId: selectedUserIds[0] || undefined,
            action: selectedAction || undefined,
          }}
        />
      </Box>
    </LocalizationProvider>
  );
}
