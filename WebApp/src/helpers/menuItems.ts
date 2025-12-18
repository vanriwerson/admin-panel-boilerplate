import {
  faChartBar,
  faChartLine,
  faCogs,
  faUsers,
} from '@fortawesome/free-solid-svg-icons';
import type { MenuItem } from '../interfaces';
import { PERMISSIONS } from '../permissions/tokens';

export const menuItems: MenuItem[] = [
  {
    label: 'Dashboard',
    icon: faChartLine,
    route: '/dashboard',
  },
  {
    label: 'Usuários',
    icon: faUsers,
    route: '/users',
    permission: PERMISSIONS.USERS,
  },
  {
    label: 'Recursos',
    icon: faCogs,
    route: '/resources',
    permission: PERMISSIONS.RESOURCES,
  },
  {
    label: 'Relatórios',
    icon: faChartBar,
    route: '/reports',
    permission: PERMISSIONS.REPORTS,
  },
];
