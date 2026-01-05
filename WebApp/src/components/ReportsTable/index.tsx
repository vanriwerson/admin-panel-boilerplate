import React, { useEffect, useState } from 'react';
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
  IconButton,
} from '@mui/material';

import type { SystemLog, SystemLogFiltersPayload } from '../../interfaces';
import { useReports } from '../../hooks/useReports';
import LogDetailsModal from '../LogDetailsModal';
import { Visibility } from '@mui/icons-material';
import NoResultsFound from '../NoResultsFound';

interface ReportsTableProps {
  filters: SystemLogFiltersPayload;
}

export default function ReportsTable({ filters }: ReportsTableProps) {
  const { logs, pagination, setPagination, setReportFilters } = useReports();
  const [selectedLog, setSelectedLog] = useState<SystemLog | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

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

  const handleViewDetails = (log: SystemLog) => {
    setSelectedLog(log);
    setModalOpen(true);
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setSelectedLog(null);
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
              <TableCell align="right">Ações</TableCell>
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
                  <TableCell align="right">
                    {log.usedPayload && (
                      <IconButton
                        onClick={() => handleViewDetails(log)}
                        title="Ver detalhes do log"
                      >
                        <Visibility />
                      </IconButton>
                    )}
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={5} align="center">
                  <NoResultsFound entity="log" />
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
      <LogDetailsModal
        open={modalOpen}
        log={selectedLog}
        onClose={handleCloseModal}
      />
    </Paper>
  );
}
