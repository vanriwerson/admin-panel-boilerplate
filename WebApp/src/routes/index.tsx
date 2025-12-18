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
import { PermissionsMap } from '../permissions/PermissionsMap';
import { CleanLayout, DefaultLayout } from '../layouts';
import { UsersProvider } from '../contexts';

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
    requiredPermission: PermissionsMap.USERS,
  },
  {
    path: '/resources',
    element: <Resources />,
    requiredPermission: PermissionsMap.RESOURCES,
  },
  {
    path: '/reports',
    element: (
      <UsersProvider>
        <Reports />
      </UsersProvider>
    ),
    requiredPermission: PermissionsMap.REPORTS,
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
