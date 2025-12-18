import { Box, Paper, Typography } from '@mui/material';

interface JsonWrapperProps {
  title: string;
  jsonContent: string;
}

export default function JsonWrapper({ title, jsonContent }: JsonWrapperProps) {
  return (
    <Box>
      <Typography variant="subtitle1" mb={2}>
        {title}
      </Typography>
      <Paper
        elevation={7}
        sx={{
          backgroundColor: '#e7e7e7ff',
          color: 'black',
          fontFamily: 'monospace',
          fontSize: '0.8rem',
          overflow: 'auto',
          p: 2,
          whiteSpace: 'pre',
        }}
      >
        {jsonContent}
      </Paper>
    </Box>
  );
}
