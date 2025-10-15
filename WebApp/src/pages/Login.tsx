import { Box, Typography, Paper } from '@mui/material';
import { LoginForm } from '../components';

const Login = () => {
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
      </Paper>
    </Box>
  );
};

export default Login;
