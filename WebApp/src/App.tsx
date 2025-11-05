import { RouterProvider } from 'react-router-dom';
import router from './routes';
import { AuthProvider, ThemeModeProvider } from './contexts';

export default function App() {
  return (
    <ThemeModeProvider>
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </ThemeModeProvider>
  );
}
