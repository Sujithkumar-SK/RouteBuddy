import { Alert, CircularProgress, Container } from '@mui/material';
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
        <div className="loading-container">
          <CircularProgress />
          <span style={{ marginLeft: '1rem' }}>Searching buses...</span>
        </div>
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
      <div className="search-summary fade-in">
        <h2 className="search-summary-title">
          {searchParams.source} → {searchParams.destination}
        </h2>
        <p className="search-summary-info">
          {new Date(searchParams.travelDate).toLocaleDateString('en-US', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
          })} • {filteredResults.length} buses found
        </p>
      </div>

      <div className="results-grid">
        {/* Filters Sidebar */}
        <div>
          <BusFilters />
        </div>

        {/* Bus Results */}
        <div>
          {filteredResults.length === 0 ? (
            <Alert severity="info" sx={{ borderRadius: '12px' }}>
              No buses found matching your criteria. Try adjusting your filters.
            </Alert>
          ) : (
            <div>
              {filteredResults.map((bus) => (
                <BusCard
                  key={bus.scheduleId}
                  bus={bus}
                  onViewSeats={onViewSeats}
                />
              ))}
            </div>
          )}
        </div>
      </div>
    </Container>
  );
};

export default BusList;