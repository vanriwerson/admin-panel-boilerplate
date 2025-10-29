import {
  Box,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
} from '@mui/material';
import AuthUserDisplay from '../AuthUserDisplay';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { useAuth } from '../../hooks';
import { filterMenuByPermissions } from '../../permissions/MenuVisibility'; // ✅ novo helper centralizado

interface SidePanelProps {
  open: boolean;
  onClose: () => void;
  onNavigate: (route: string) => void;
}

const drawerWidth = 260;

export default function SidePanel({
  open,
  onClose,
  onNavigate,
}: SidePanelProps) {
  const { authUser, handleLogout } = useAuth();

  // ✅ Filtra os itens do menu de acordo com as permissões do usuário logado
  const filteredMenu = filterMenuByPermissions(authUser);

  return (
    <Drawer
      variant="persistent"
      open={open}
      onClose={onClose}
      sx={{
        width: drawerWidth,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
        },
      }}
    >
      <Toolbar />
      <Box sx={{ overflow: 'auto' }}>
        <AuthUserDisplay />

        <List>
          {filteredMenu.map((item) => (
            <ListItem key={item.label} disablePadding>
              <ListItemButton onClick={() => onNavigate(item.route)}>
                <ListItemIcon>
                  <FontAwesomeIcon icon={item.icon} />
                </ListItemIcon>
                <ListItemText primary={item.label} />
              </ListItemButton>
            </ListItem>
          ))}

          <ListItemButton
            onClick={() => {
              handleLogout();
              onNavigate('/login');
            }}
          >
            <ListItemIcon>
              <FontAwesomeIcon icon={faSignOutAlt} />
            </ListItemIcon>
            <ListItemText primary="Sair" />
          </ListItemButton>
        </List>
      </Box>
    </Drawer>
  );
}
