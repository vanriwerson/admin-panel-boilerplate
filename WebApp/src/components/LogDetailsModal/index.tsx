import { Modal, Box, Typography, Paper, IconButton } from '@mui/material';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faClose, faEye } from '@fortawesome/free-solid-svg-icons';
import type { SystemLog } from '../../interfaces';
import { useEffect, useState } from 'react';
import { detectAndFetchEntity, getErrorMessage } from '../../helpers';
import { useNotification } from '../../hooks';

interface Props {
  open: boolean;
  log: SystemLog | null;
  onClose: () => void;
}

function formatPayload(payload: string): string {
  try {
    const parsed = JSON.parse(payload);
    return JSON.stringify(parsed, null, 2);
  } catch {
    return payload;
  }
}

export default function LogDetailsModal({ open, log, onClose }: Props) {
  const { showNotification } = useNotification();
  const [entity, setEntity] = useState<unknown | null>(null);

  useEffect(() => {
    if (!log || !log.usedPayload || !log.action.includes('update')) {
      setEntity(null);
      return;
    }

    const run = async () => {
      try {
        const payload = JSON.parse(log.usedPayload || '');

        const payloadEntity = await detectAndFetchEntity(payload);
        setEntity(payloadEntity);
      } catch (err) {
        showNotification(getErrorMessage(err), 'warning');
        setEntity(null);
      }
    };

    run();
  }, [log, showNotification]);

  if (!log) return null;

  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: { xs: '90%', sm: 600, md: 840 },
          maxHeight: '90vh',
          overflow: 'auto',
        }}
      >
        <Paper sx={{ p: 3 }}>
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mb={2}
          >
            <Typography variant="h6" component="h2">
              <FontAwesomeIcon icon={faEye} style={{ marginRight: 8 }} />
              Detalhes do Log {log.id}
            </Typography>
            <IconButton onClick={onClose}>
              <FontAwesomeIcon icon={faClose} />
            </IconButton>
          </Box>

          <Box sx={{ mb: 3 }}>
            <Box
              sx={{
                display: 'flex',
                gap: 2,
                mb: 2,
                width: '100%',
              }}
            >
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Usuário
                </Typography>
                <Typography variant="body1">
                  {log.user.fullName} ({log.user.username})
                </Typography>
              </Box>
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Ação
                </Typography>
                <Typography variant="body1">{log.action}</Typography>
              </Box>
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Data/Hora
                </Typography>
                <Typography variant="body1">
                  {new Date(log.createdAt).toLocaleString()}
                </Typography>
              </Box>
            </Box>
          </Box>

          <Box display="flex" width="100%" justifyContent="center" gap={2}>
            {log.usedPayload && (
              <Box>
                <Typography variant="subtitle2">
                  {log.action.includes('create')
                    ? 'Payload Utilizado'
                    : 'Estado anterior'}
                </Typography>
                <Paper
                  sx={{
                    p: 2,
                    fontFamily: 'monospace',
                    fontSize: '0.8rem',
                    whiteSpace: 'pre',
                    overflow: 'auto',
                  }}
                >
                  {formatPayload(log.usedPayload)}
                </Paper>
              </Box>
            )}

            {entity ? (
              <Box>
                <Typography variant="subtitle2">Estado atual</Typography>
                <Paper
                  sx={{
                    p: 2,
                    fontFamily: 'monospace',
                    fontSize: '0.8rem',
                    whiteSpace: 'pre',
                    overflow: 'auto',
                  }}
                >
                  {JSON.stringify(entity, null, 2)}
                </Paper>
              </Box>
            ) : null}
          </Box>
        </Paper>
      </Box>
    </Modal>
  );
}
