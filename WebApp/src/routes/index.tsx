import { createBrowserRouter } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import {
  DashBoard,
  Login,
  NotFound,
  PasswordReset,
  Reports,
  Resources,
  UnauthorizedAccess,
  Users,
} from '../pages';
import { CleanLayout, DefaultLayout } from '../layouts';
import { UsersProvider } from '../contexts';
import { PERMISSIONS } from '../permissions';

const publicRoutes = [
  { path: '/login', element: <Login /> },
  { path: '/password-reset', element: <PasswordReset /> },
];

const privateRoutes = [
  {
    path: '/dashboard',
    element: <DashBoard />,
  },
  {
    path: '/users',
    element: (
      <UsersProvider>
        <Users />
      </UsersProvider>
    ),
    requiredPermission: PERMISSIONS.USERS,
  },
  {
    path: '/resources',
    element: <Resources />,
    requiredPermission: PERMISSIONS.RESOURCES,
  },
  {
    path: '/reports',
    element: (
      <UsersProvider>
        <Reports />
      </UsersProvider>
    ),
    requiredPermission: PERMISSIONS.REPORTS,
  },
];

const protectedRoutes = privateRoutes.map((route) => ({
  path: route.path,
  element: (
    <ProtectedRoute requiredPermission={route.requiredPermission}>
      {route.element}
    </ProtectedRoute>
  ),
}));

const router = createBrowserRouter([
  {
    element: <CleanLayout />,
    children: [
      { path: '/', element: <Login /> },
      ...publicRoutes,
      { path: '/unauthorized', element: <UnauthorizedAccess /> },
      { path: '*', element: <NotFound /> },
    ],
  },

  {
    element: <DefaultLayout />,
    children: protectedRoutes,
  },
]);

export default router;
