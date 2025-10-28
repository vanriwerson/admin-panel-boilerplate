import {
  faChartBar,
  faCogs,
  faUserAlt,
  faUsers,
} from '@fortawesome/free-solid-svg-icons';
import type { MenuItem } from '../interfaces';

export const menuItems: MenuItem[] = [
  { label: 'Perfil', icon: faUserAlt, route: '/profile' },
  { label: 'Usuários', icon: faUsers, route: '/users' },
  { label: 'Recursos', icon: faCogs, route: '/resources' },
  { label: 'Relatórios', icon: faChartBar, route: '/reports' },
];
