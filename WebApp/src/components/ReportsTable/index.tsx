import React, { useEffect } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TableContainer,
  Paper,
  TablePagination,
  Typography,
  Box,
} from '@mui/material';

import type { SystemLog, SystemLogFiltersPayload } from '../../interfaces';
import { useReports } from '../../hooks/useReports';

interface ReportsTableProps {
  filters: SystemLogFiltersPayload;
}

export default function ReportsTable({ filters }: ReportsTableProps) {
  const { logs, pagination, setPagination, setReportFilters } = useReports();

  useEffect(() => {
    setReportFilters({
      ...filters,
      page: 1,
      pageSize: pagination.pageSize,
    });
  }, [filters, pagination.pageSize, setReportFilters]);

  const handleChangePage = (_: unknown, newPage: number) => {
    setPagination((prev) => ({
      ...prev,
      page: newPage + 1,
    }));
  };

  const handleChangeRowsPerPage = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPagination({
      ...pagination,
      page: 1,
      pageSize: parseInt(e.target.value, 10),
    });
  };

  return (
    <Paper sx={{ p: 2 }}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h6">Logs do Sistema</Typography>
      </Box>

      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Usuário</TableCell>
              <TableCell>Ação</TableCell>
              <TableCell>Data</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {logs.length > 0 ? (
              logs.map((log: SystemLog) => (
                <TableRow key={log.id} hover>
                  <TableCell>{log.id}</TableCell>
                  <TableCell>{log.user.fullName}</TableCell>
                  <TableCell>{log.action}</TableCell>
                  <TableCell>
                    {new Date(log.createdAt).toLocaleString()}
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={4} align="center">
                  Nenhum log encontrado
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Box display="flex" justifyContent="flex-end">
        <TablePagination
          component="div"
          count={pagination.totalItems}
          page={pagination.page - 1}
          onPageChange={handleChangePage}
          rowsPerPage={pagination.pageSize}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Itens por página:"
          rowsPerPageOptions={[5, 10, 25]}
        />
      </Box>
    </Paper>
  );
}
