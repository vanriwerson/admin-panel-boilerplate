import { RouterProvider } from 'react-router-dom';
import router from './routes';
import {
  AuthProvider,
  ThemeModeProvider,
  NotificationProvider,
  PermissionsProvider,
  SystemResourcesProvider,
} from './contexts';
import { SnackbarNotification } from './components';

export default function App() {
  return (
    <ThemeModeProvider>
      <NotificationProvider>
        <AuthProvider>
          <SystemResourcesProvider>
            <PermissionsProvider>
              <RouterProvider router={router} />
              <SnackbarNotification />
            </PermissionsProvider>
          </SystemResourcesProvider>
        </AuthProvider>
      </NotificationProvider>
    </ThemeModeProvider>
  );
}
