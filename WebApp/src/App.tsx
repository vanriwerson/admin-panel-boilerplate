import React, { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { Box, IconButton, Toolbar } from '@mui/material';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars } from '@fortawesome/free-solid-svg-icons';
import { SidePanel } from './components';

const App: React.FC = () => {
  const [open, setOpen] = useState(true);
  const navigate = useNavigate();

  const handleNavigate = (route: string) => {
    navigate(route); // navegação do react-router
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <SidePanel
        open={open}
        onClose={() => setOpen(false)}
        onNavigate={handleNavigate}
      />

      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar>
          <IconButton onClick={() => setOpen(!open)}>
            <FontAwesomeIcon icon={faBars} />
          </IconButton>
        </Toolbar>

        {/* Aqui é renderizado o conteúdo da rota */}
        <Outlet />
      </Box>
    </Box>
  );
};

export default App;
