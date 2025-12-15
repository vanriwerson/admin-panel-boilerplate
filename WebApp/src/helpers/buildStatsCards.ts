import type { SystemStats } from '../interfaces';
import { menuItems } from './menuItems';

export function buildStatsCards(systemStats: SystemStats) {
  const {
    usersCount,
    systemResourcesCount,
    monthlyReportsCount,
    monthlyReportsCountReference,
  } = systemStats;

  return [
    {
      bg: '#6dc4edff, #215fb0ff',
      icon: menuItems[2].icon,
      content: `${usersCount} Usuários Ativos`,
    },
    {
      bg: '#cc2b5e, #753a88',
      icon: menuItems[3].icon,
      content: `${systemResourcesCount} Recursos de Sistema`,
    },
    {
      bg: '#734b6d, #42275a',
      icon: menuItems[4].icon,
      content: `${monthlyReportsCount} Ações auditadas em ${monthlyReportsCountReference}`,
    },
  ];
}
