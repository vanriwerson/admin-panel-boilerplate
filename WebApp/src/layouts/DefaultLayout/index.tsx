import { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { Box, IconButton, Toolbar } from '@mui/material';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars } from '@fortawesome/free-solid-svg-icons';
import { SidePanel } from '../../components';

export default function DefaultLayout() {
  const [open, setOpen] = useState(true);
  const navigate = useNavigate();

  const handleNavigate = (route: string) => {
    navigate(route);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <SidePanel open={open} onNavigate={handleNavigate} />

      <Box component="main" sx={{ flexGrow: 1 }}>
        <Toolbar>
          <IconButton onClick={() => setOpen(!open)}>
            <FontAwesomeIcon icon={faBars} />
          </IconButton>
        </Toolbar>
        <Outlet />
      </Box>
    </Box>
  );
}
