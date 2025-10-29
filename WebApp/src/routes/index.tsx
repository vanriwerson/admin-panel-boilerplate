import { createBrowserRouter } from 'react-router-dom';
import App from '../App';
import ProtectedRoute from '../routes/ProtectedRoute';
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

const publicRoutes = [
  { path: '/', element: <Login /> },
  { path: '/login', element: <Login /> },
  { path: '/password-reset', element: <PasswordReset /> },
];

const privateRoutes = [
  { path: '/profile', element: <Profile /> },
  { path: '/unauthorized', element: <UnauthorizedAccess /> },
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
  ...publicRoutes,
  {
    path: '/', // layout com SidePanel
    element: <App />,
    children: protectedRoutes,
  },
]);

export default router;
