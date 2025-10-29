import { useState, useEffect } from 'react';
import { Box, Typography, Paper, Link, CircularProgress } from '@mui/material';
import { LoginForm, PasswordResetRequestModal } from '../../components';
import { useAuth } from '../../hooks';
import { useLocation, useNavigate } from 'react-router-dom';

export default function Login() {
  const [openResetModal, setOpenResetModal] = useState(false);
  const [isCheckingToken, setIsCheckingToken] = useState(true);

  const { token, handleExternalLogin } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const searchParams = new URLSearchParams(location.search);
    const urlToken = searchParams.get('token');

    async function tryExternalLogin() {
      if (urlToken) {
        try {
          await handleExternalLogin({ externalToken: urlToken });
          navigate('/profile', { replace: true });
          return;
        } catch (error) {
          console.error('Erro no login externo:', error);
        }
      }

      if (token) {
        navigate('/profile', { replace: true });
        return;
      }

      setIsCheckingToken(false);
    }

    void tryExternalLogin();
  }, [location.search, token, handleExternalLogin, navigate]);

  if (isCheckingToken) {
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
        <CircularProgress />
      </Box>
    );
  }

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
