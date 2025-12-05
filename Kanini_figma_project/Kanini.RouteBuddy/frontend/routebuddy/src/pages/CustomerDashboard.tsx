import { Container } from '@mui/material';
import Layout from '../components/layout/Layout';
import BusSearch from '../features/bus/BusSearch';

const CustomerDashboard = () => {
  const handleViewSeats = (scheduleId: number) => {
    console.log('View seats for schedule:', scheduleId);
    // Will be implemented in Phase 3
  };

  return (
    <Layout>
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <BusSearch />
      </Container>
    </Layout>
  );
};

export default CustomerDashboard;
