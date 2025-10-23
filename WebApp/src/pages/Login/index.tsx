import { useState } from 'react';
import { Box, Typography, Paper, Link } from '@mui/material';
import { LoginForm, PasswordResetRequestModal } from '../../components';

export default function Login() {
  const [openResetModal, setOpenResetModal] = useState(false);

  return (
    <Box
      sx={{
        height: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: '#f5f5f5',
      }}
    >
      <Paper
        elevation={3}
        sx={{
          padding: 4,
          width: '100%',
          maxWidth: 400,
          borderRadius: 2,
        }}
      >
        <Typography variant="h5" align="center" gutterBottom>
          Faça login no sistema
        </Typography>

        <LoginForm />

        <Box mt={2} textAlign="center">
          <Link
            component="button"
            variant="body2"
            onClick={() => setOpenResetModal(true)}
          >
            Esqueci minha senha
          </Link>
        </Box>
      </Paper>

      <PasswordResetRequestModal
        open={openResetModal}
        onClose={() => setOpenResetModal(false)}
      />
    </Box>
  );
}
