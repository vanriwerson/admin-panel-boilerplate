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
import { menuItems } from '../../helpers';

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
          {menuItems.map((item) => (
            <ListItem key={item.label} disablePadding>
              <ListItemButton onClick={() => onNavigate(item.route)}>
                <ListItemIcon>
                  <FontAwesomeIcon icon={item.icon} />
                </ListItemIcon>
                <ListItemText primary={item.label} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Box>
    </Drawer>
  );
}
