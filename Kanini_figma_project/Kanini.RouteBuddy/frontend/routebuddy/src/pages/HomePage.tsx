import { Container, Typography, Box } from '@mui/material';
import Layout from '../components/layout/Layout';
import BusSearch from '../features/bus/BusSearch';

const HomePage = () => {
  return (
    <Layout>
      <Box sx={{ 
        minHeight: '100vh', 
        bgcolor: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        display: 'flex',
        alignItems: 'center'
      }}>
        <Container maxWidth="md">
          <Box sx={{ textAlign: 'center', mb: 4 }}>
            <Typography variant="h2" sx={{ 
              fontWeight: 700, 
              color: 'white', 
              mb: 2,
              textShadow: '2px 2px 4px rgba(0,0,0,0.3)'
            }}>
              🚌 ROUTEBUDDY
            </Typography>
            <Typography variant="h5" sx={{ 
              color: 'white', 
              mb: 4,
              opacity: 0.9
            }}>
              Book Your Journey
            </Typography>
          </Box>
          
          <Box sx={{ 
            bgcolor: 'white', 
            borderRadius: 2, 
            p: 3,
            boxShadow: '0 8px 32px rgba(0,0,0,0.1)'
          }}>
            <BusSearch />
          </Box>
        </Container>
      </Box>
    </Layout>
  );
};

export default HomePage;