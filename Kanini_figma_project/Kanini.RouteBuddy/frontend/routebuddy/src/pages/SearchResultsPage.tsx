import { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Box } from '@mui/material';
import { useAppDispatch, useAppSelector } from '../hooks/useAppDispatch';
import { searchBuses } from '../features/bus/busSlice';
import BusSearch from '../features/bus/BusSearch';
import BusList from '../features/bus/BusList';
import Layout from '../components/layout/Layout';
import { ROUTES } from '../utils/constants';

const SearchResultsPage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { searchParams } = useAppSelector((state) => state.bus);

  useEffect(() => {
    // If search params are passed via navigation state, trigger search
    if (location.state?.searchParams) {
      dispatch(searchBuses(location.state.searchParams));
    }
  }, [dispatch, location.state]);

  const handleViewSeats = (scheduleId: number) => {
    // Navigate to seat selection page with travel date
    const travelDate = searchParams?.travelDate || new Date().toISOString().split('T')[0];
    navigate(`/seat-selection/${scheduleId}`, {
      state: { scheduleId, travelDate }
    });
  };

  return (
    <Layout>
      <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50' }}>
        {/* Search Form - Always visible at top */}
        <Box sx={{ bgcolor: 'white', borderBottom: 1, borderColor: 'divider' }}>
          <BusSearch />
        </Box>

        {/* Search Results */}
        <BusList onViewSeats={handleViewSeats} />
      </Box>
    </Layout>
  );
};

export default SearchResultsPage;