import { createTheme } from '@mui/material/styles';

export const defineTheme = (mode: 'light' | 'dark') =>
  createTheme({
    palette: {
      mode,
      ...(mode === 'light'
        ? {
            // 🎨 Cores do modo claro
            primary: { main: '#198a0fff' },
            background: { default: '#f5f5f5', paper: '#fff' },
            text: { primary: '#000', secondary: '#333' },
          }
        : {
            // 🌙 Cores do modo escuro
            primary: { main: '#b6f990ff' },
            background: { default: '#121212', paper: '#1e1e1e' },
            text: { primary: '#fff', secondary: '#ccc' },
          }),
    },
    typography: {
      fontFamily:
        'Roboto, system-ui, -apple-system, "Segoe UI", Helvetica, Arial, sans-serif',
    },
  });
