import { createBrowserRouter } from 'react-router-dom';
import App from '../App';
import {
  Dashboard,
  Login,
  PasswordReset,
  Users,
  Resources,
  Reports,
} from '../pages';

const publicRoutes = [
  { path: '/', element: <Login /> },
  { path: '/login', element: <Login /> },
  { path: '/password-reset', element: <PasswordReset /> },
];

const privateRoutes = [
  { path: '/dashboard', element: <Dashboard /> },
  { path: '/users', element: <Users /> },
  { path: '/resources', element: <Resources /> },
  { path: '/reports', element: <Reports /> },
];

const router = createBrowserRouter([
  ...publicRoutes,

  // Layout com SidePanel
  {
    path: '/',
    element: <App />,
    children: privateRoutes,
  },
]);

export default router;
