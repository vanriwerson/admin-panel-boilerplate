import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Typography, Box } from '@mui/material';
import { pageTitleIcons } from '../../helpers';

interface PageTitleProps {
  icon?: string;
  title: string;
}

export default function PageTitle({ icon, title }: PageTitleProps) {
  return (
    <Box
      sx={{
        width: '100%',
        textAlign: 'left',
        mb: 2,
      }}
    >
      <Typography variant="h3" display="flex" alignItems="center" gap={2}>
        {icon && <FontAwesomeIcon icon={pageTitleIcons[icon]} size="sm" />}
        {title}
      </Typography>
    </Box>
  );
}
