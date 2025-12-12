import { RouterProvider } from 'react-router-dom';
import router from './routes';
import { AuthProvider, ThemeModeProvider, UsersProvider } from './contexts';

export default function App() {
  return (
    <ThemeModeProvider>
      <AuthProvider>
        <UsersProvider>
          <RouterProvider router={router} />
        </UsersProvider>
      </AuthProvider>
    </ThemeModeProvider>
  );
}
