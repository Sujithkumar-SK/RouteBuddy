import { Box, Typography, Alert, CircularProgress, Container, Grid } from '@mui/material';
import { useAppSelector } from '../../hooks/useAppDispatch';
import BusCard from '../../components/ui/BusCard';
import BusFilters from './BusFilters';

interface BusListProps {
  onViewSeats: (scheduleId: number) => void;
}

const BusList = ({ onViewSeats }: BusListProps) => {
  const { filteredResults, loading, error, searchParams } = useAppSelector((state) => state.bus);

  if (loading) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 200 }}>
          <CircularProgress />
          <Typography sx={{ ml: 2 }}>Searching buses...</Typography>
        </Box>
      </Container>
    );
  }

  if (error) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  if (!searchParams) {
    return (
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Alert severity="info">Please search for buses to see results.</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      {/* Search Summary */}
      <Box sx={{ mb: 3 }}>
        <Typography variant="h5" sx={{ fontWeight: 600, mb: 1 }}>
          {searchParams.source} → {searchParams.destination}
        </Typography>
        <Typography variant="body1" color="text.secondary">
          {new Date(searchParams.travelDate).toLocaleDateString('en-US', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
          })} • {filteredResults.length} buses found
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {/* Filters Sidebar */}
        <Grid item xs={12} md={3}>
          <BusFilters />
        </Grid>

        {/* Bus Results */}
        <Grid item xs={12} md={9}>
          {filteredResults.length === 0 ? (
            <Alert severity="info">
              No buses found matching your criteria. Try adjusting your filters.
            </Alert>
          ) : (
            <Box>
              {filteredResults.map((bus) => (
                <BusCard
                  key={bus.scheduleId}
                  bus={bus}
                  onViewSeats={onViewSeats}
                />
              ))}
            </Box>
          )}
        </Grid>
      </Grid>
    </Container>
  );
};

export default BusList;