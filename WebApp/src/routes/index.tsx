import { createBrowserRouter } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import {
  Login,
  PasswordReset,
  Profile,
  Reports,
  Resources,
  UnauthorizedAccess,
  Users,
} from '../pages';
import { PermissionsMap } from '../permissions/PermissionsMap';
import { CleanLayout, DefaultLayout } from '../layouts';

const publicRoutes = [
  { path: '/login', element: <Login /> },
  { path: '/password-reset', element: <PasswordReset /> },
];

const privateRoutes = [
  { path: '/profile', element: <Profile /> },
  {
    path: '/users',
    element: <Users />,
    requiredPermission: PermissionsMap.USERS,
  },
  {
    path: '/resources',
    element: <Resources />,
    requiredPermission: PermissionsMap.RESOURCES,
  },
  {
    path: '/reports',
    element: <Reports />,
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
    ],
  },

  {
    element: <DefaultLayout />,
    children: protectedRoutes,
  },
]);

export default router;
