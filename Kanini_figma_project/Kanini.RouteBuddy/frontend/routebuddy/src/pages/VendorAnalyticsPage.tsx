import { Container, Typography, Box } from '@mui/material';
import Layout from '../components/layout/Layout';

const VendorAnalyticsPage = () => {
  return (
    <Layout>
      <Container maxWidth="xl">
        <Box sx={{ py: 3 }}>
          <Typography variant="h4" gutterBottom>
            Analytics & Reports
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Revenue analytics and performance metrics will be displayed here.
          </Typography>
        </Box>
      </Container>
    </Layout>
  );
};

export default VendorAnalyticsPage;