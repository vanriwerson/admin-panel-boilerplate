import type { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import type { SystemStats } from '../interfaces';

export function buildStatsCards(
  systemStats: SystemStats,
  cardIcon: Record<string, IconDefinition>
) {
  const {
    usersCount,
    systemResourcesCount,
    monthlyReportsCount,
    monthlyReportsCountReference,
  } = systemStats;

  return [
    {
      bg: '#6dc4edff, #215fb0ff',
      icon: cardIcon.users,
      content: `${usersCount} Usuários Ativos`,
    },
    {
      bg: '#cc2b5e, #753a88',
      icon: cardIcon.systemResources,
      content: `${systemResourcesCount} Recursos de Sistema`,
    },
    {
      bg: '#fdc426ff, #e67e22',
      icon: cardIcon.reports,
      content: `${monthlyReportsCount} Ações auditadas em ${monthlyReportsCountReference}`,
    },
  ];
}
