import { Container, Typography } from '@mui/material';
import Layout from '../components/layout/Layout';

const VendorDashboard = () => {
  return (
    <Layout>
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Typography variant="h4" gutterBottom>
          Vendor Dashboard
        </Typography>
        <Typography variant="body1">
          Welcome to your vendor dashboard!
        </Typography>
      </Container>
    </Layout>
  );
};

export default VendorDashboard;
