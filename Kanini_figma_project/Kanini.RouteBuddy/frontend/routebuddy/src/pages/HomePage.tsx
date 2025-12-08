import { Container, Box } from '@mui/material';
import Layout from '../components/layout/Layout';
import BusSearch from '../features/bus/BusSearch';

const HomePage = () => {
  return (
    <Layout>
      <Box className="home-container">
        <Container maxWidth="md">
          <div className="home-header fade-in">
            <h1 className="home-title">🚌 ROUTEBUDDY</h1>
            <p className="home-subtitle">Book Your Journey with Comfort & Ease</p>
          </div>
          
          <div className="home-search-card fade-in">
            <BusSearch />
          </div>
        </Container>
      </Box>
    </Layout>
  );
};

export default HomePage;