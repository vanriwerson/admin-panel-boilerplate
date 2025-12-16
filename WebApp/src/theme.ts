import { createTheme } from '@mui/material/styles';

export const defineTheme = (mode: 'light' | 'dark') =>
  createTheme({
    palette: {
      mode,
      ...(mode === 'light'
        ? {
            // 🎨 Cores do modo claro
            primary: {
              main: '#198a0fff',
              light: '#4caf50',
              dark: '#0d6526',
              contrastText: '#000',
            },
            secondary: {
              main: '#8340ffff',
              light: '#af71ffff',
              dark: '#7700ffff',
              contrastText: '#000',
            },
            success: {
              main: '#4caf50', // Verde sucesso
              light: '#81c784',
              dark: '#388e3c',
            },
            error: {
              main: '#f44336', // Vermelho erro
              light: '#e57373',
              dark: '#d32f2f',
            },
            warning: {
              main: '#ff9800', // Laranja aviso
              light: '#ffb74d',
              dark: '#f57c00',
            },
            info: {
              main: '#2196f3', // Azul informação
              light: '#64b5f6',
              dark: '#1976d2',
            },
            background: { default: '#f5f5f5', paper: '#fff' },
            text: { primary: '#000', secondary: '#333' },
          }
        : {
            // 🌙 Cores do modo escuro
            primary: {
              main: '#198a0fff',
              light: '#4caf50',
              dark: '#0d6526',
              contrastText: '#fff',
            },
            secondary: {
              main: '#6f00ffff',
              light: '#9a4dffff',
              dark: '#6b00e6ff',
              contrastText: '#fff',
            },
            success: {
              main: '#66bb6a', // Verde sucesso escuro
              light: '#98ee99',
              dark: '#43a047',
            },
            error: {
              main: '#f44336', // Vermelho erro (igual ao claro)
              light: '#e57373',
              dark: '#d32f2f',
            },
            warning: {
              main: '#ffa726', // Laranja aviso escuro
              light: '#ffb74d',
              dark: '#f57c00',
            },
            info: {
              main: '#42a5f5', // Azul informação escuro
              light: '#80d6ff',
              dark: '#1976d2',
            },
            background: { default: '#121212', paper: '#1e1e1e' },
            text: { primary: '#fff', secondary: '#ccc' },
          }),
    },
    typography: {
      fontFamily:
        'Roboto, system-ui, -apple-system, "Segoe UI", Helvetica, Arial, sans-serif',
    },
  });
