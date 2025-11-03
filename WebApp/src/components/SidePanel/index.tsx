import {
  Box,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Tooltip,
} from '@mui/material';
import AuthUserDisplay from '../AuthUserDisplay';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { useAuth } from '../../hooks';
import { filterMenuByPermissions } from '../../permissions/MenuVisibility';

interface SidePanelProps {
  open: boolean;
  onToggle: () => void;
  onNavigate: (route: string) => void;
}

const drawerWidth = 260;
const collapsedWidth = 52;

export default function SidePanel({ open, onNavigate }: SidePanelProps) {
  const { authUser, handleLogout } = useAuth();

  const filteredMenu = filterMenuByPermissions(authUser);

  return (
    <Drawer
      variant="permanent"
      open={open}
      sx={{
        width: open ? drawerWidth : collapsedWidth,
        flexShrink: 0,
        whiteSpace: 'nowrap',
        transition: (theme) =>
          theme.transitions.create('width', {
            easing: theme.transitions.easing.sharp,
            duration: theme.transitions.duration.enteringScreen,
          }),
        '& .MuiDrawer-paper': {
          width: open ? drawerWidth : collapsedWidth,
          transition: (theme) =>
            theme.transitions.create('width', {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen,
            }),
        },
      }}
    >
      <Box sx={{ overflow: 'hidden' }}>
        <AuthUserDisplay collapsed={!open} />

        <List>
          {filteredMenu.map((item) => (
            <ListItem key={item.label} disablePadding>
              <Tooltip
                title={item.label}
                placement="right"
                arrow
                disableHoverListener={open}
              >
                <ListItemButton onClick={() => onNavigate(item.route)}>
                  <ListItemIcon>
                    <FontAwesomeIcon icon={item.icon} />
                  </ListItemIcon>
                  <ListItemText
                    primary={item.label}
                    sx={{
                      opacity: open ? 1 : 0,
                      transition: 'opacity 0.3s',
                      whiteSpace: 'nowrap',
                    }}
                  />
                </ListItemButton>
              </Tooltip>
            </ListItem>
          ))}

          <ListItem disablePadding>
            <Tooltip
              title="Sair"
              placement="right"
              arrow
              disableHoverListener={open}
            >
              <ListItemButton
                onClick={() => {
                  handleLogout();
                  onNavigate('/login');
                }}
              >
                <ListItemIcon>
                  <FontAwesomeIcon icon={faSignOutAlt} />
                </ListItemIcon>
                <ListItemText
                  primary="Sair"
                  sx={{
                    opacity: open ? 1 : 0,
                    transition: 'opacity 0.3s',
                    whiteSpace: 'nowrap',
                  }}
                />
              </ListItemButton>
            </Tooltip>
          </ListItem>
        </List>
      </Box>
    </Drawer>
  );
}
