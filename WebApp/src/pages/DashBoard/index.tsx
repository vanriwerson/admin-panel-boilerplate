import { Box, CircularProgress, Container } from '@mui/material';
import {
  DashboardDevelopmentTips,
  PageTitle,
  StatsCard,
} from '../../components';
import { useEffect, useState, useContext } from 'react';
import type { SystemStats } from '../../interfaces';
import { getSystemStats } from '../../services';
import { buildStatsCards } from '../../helpers';
import { ThemeContext } from '../../contexts';

export default function DashBoard() {
  const [systemStats, setSystemStats] = useState<SystemStats | null>(null);
  const { mode } = useContext(ThemeContext)!;

  useEffect(() => {
    async function fetchStats() {
      try {
        const stats = await getSystemStats();
        setSystemStats(stats);
      } catch (err) {
        console.error(err);
        alert('❌ Erro ao carregar estatísticas do sistema');
      }
    }

    fetchStats();
  }, []);

  const getBg = (originalBg: string) => {
    if (mode === 'dark') {
      return originalBg.split(', ').reverse().join(', ');
    }
    return originalBg;
  };

  const statsCards = systemStats ? buildStatsCards(systemStats) : [];

  return (
    <Container
      sx={{
        mt: 4,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}
    >
      <PageTitle icon="dashboard" title="Dashboard" />

      {systemStats === null ? (
        <CircularProgress />
      ) : (
        <Box display="flex" gap={2} width="100%" flexWrap="wrap">
          {statsCards.map((card, index) => (
            <StatsCard
              key={index}
              background={getBg(card.bg)}
              icon={card.icon}
              content={card.content}
            />
          ))}
        </Box>
      )}

      <DashboardDevelopmentTips />
    </Container>
  );
}
