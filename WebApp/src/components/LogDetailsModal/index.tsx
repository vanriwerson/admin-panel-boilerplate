import { Modal, Box, Typography, Button, Paper } from '@mui/material';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faClose, faEye } from '@fortawesome/free-solid-svg-icons';
import type { SystemLog } from '../../interfaces';

interface Props {
  open: boolean;
  log: SystemLog | null;
  onClose: () => void;
}

function formatPayload(payload: string): string {
  try {
    // Tenta fazer parse do JSON
    const parsed = JSON.parse(payload);
    // Formata com indentação de 2 espaços
    return JSON.stringify(parsed, null, 2);
  } catch {
    // Se não for JSON válido, retorna como está
    return payload;
  }
}

export default function LogDetailsModal({ open, log, onClose }: Props) {
  if (!log) return null;

  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: { xs: '90%', sm: 600 },
          maxHeight: '80vh',
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
            <Button onClick={onClose} size="small">
              <FontAwesomeIcon icon={faClose} />
            </Button>
          </Box>

          <Box sx={{ mb: 3 }}>
            <Box
              sx={{
                display: 'grid',
                gridTemplateColumns: '1fr 1fr',
                gap: 2,
                mb: 2,
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

          {log.usedPayload && (
            <Box>
              <Typography
                variant="subtitle2"
                color="text.secondary"
                gutterBottom
              >
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
                  maxHeight: '540px',
                  overflow: 'auto',
                }}
              >
                {formatPayload(log.usedPayload)}
              </Paper>
            </Box>
          )}
        </Paper>
      </Box>
    </Modal>
  );
}
