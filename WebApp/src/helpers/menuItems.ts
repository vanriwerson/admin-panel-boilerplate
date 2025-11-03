import {
  faChartBar,
  faCogs,
  faUserAlt,
  faUsers,
} from '@fortawesome/free-solid-svg-icons';
import type { MenuItem } from '../interfaces';
import { PermissionsMap } from '../permissions/PermissionsMap';

export const menuItems: MenuItem[] = [
  {
    label: 'Perfil',
    icon: faUserAlt,
    route: '/profile',
  },
  {
    label: 'Usuários',
    icon: faUsers,
    route: '/users',
    permission: PermissionsMap.USERS,
  },
  {
    label: 'Recursos',
    icon: faCogs,
    route: '/resources',
    permission: PermissionsMap.RESOURCES,
  },
  {
    label: 'Relatórios',
    icon: faChartBar,
    route: '/reports',
    permission: PermissionsMap.REPORTS,
  },
];
