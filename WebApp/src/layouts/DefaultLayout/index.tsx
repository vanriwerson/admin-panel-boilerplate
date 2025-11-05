import { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { Box, IconButton, Toolbar } from '@mui/material';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars, faMoon, faSun } from '@fortawesome/free-solid-svg-icons';
import { SidePanel } from '../../components';
import { useThemeMode } from '../../hooks';

export default function DefaultLayout() {
  const [open, setOpen] = useState(true);
  const { mode, toggleTheme } = useThemeMode();
  const navigate = useNavigate();

  const handleNavigate = (route: string) => {
    navigate(route);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <SidePanel open={open} onNavigate={handleNavigate} />

      <Box component="main" sx={{ flexGrow: 1 }}>
        <Toolbar sx={{ justifyContent: 'space-between' }}>
          <IconButton onClick={() => setOpen(!open)}>
            <FontAwesomeIcon icon={faBars} />
          </IconButton>

          <IconButton onClick={toggleTheme}>
            <FontAwesomeIcon
              icon={mode === 'light' ? faMoon : faSun}
              size="sm"
            />
          </IconButton>
        </Toolbar>

        <Outlet />
      </Box>
    </Box>
  );
}
