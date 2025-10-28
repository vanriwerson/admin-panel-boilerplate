import { createBrowserRouter } from 'react-router-dom';
import App from '../App';
import {
  Login,
  PasswordReset,
  Profile,
  Reports,
  Resources,
  Users,
} from '../pages';

const publicRoutes = [
  { path: '/', element: <Login /> },
  { path: '/login', element: <Login /> },
  { path: '/password-reset', element: <PasswordReset /> },
];

const privateRoutes = [
  { path: '/profile', element: <Profile /> },
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
