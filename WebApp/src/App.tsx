import { RouterProvider } from 'react-router-dom';
import { CssBaseline } from '@mui/material';
import router from './routes';
import { AuthProvider, ThemeModeProvider } from './contexts';

export default function App() {
  return (
    <ThemeModeProvider>
      <CssBaseline />
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </ThemeModeProvider>
  );
}
