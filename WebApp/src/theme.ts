import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  typography: {
    fontFamily:
      'Roboto, system-ui, -apple-system, "Segoe UI", Helvetica, Arial, sans-serif',
  },
  palette: {
    primary: { main: '#198a0fff' },
    background: { default: '#f5f5f5' },
  },
});

export default theme;
