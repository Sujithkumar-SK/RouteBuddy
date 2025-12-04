import { Card, CardContent, Box, Typography, Chip, Button, Rating } from '@mui/material';
import { AccessTime, EventSeat, Wifi, Power, Restaurant } from '@mui/icons-material';
import { getBusTypeName, getAmenityNames, BusAmenities } from '../../utils/constants';
import type { BusSearchResponse } from '../../features/bus/types';

interface BusCardProps {
  bus: BusSearchResponse;
  onViewSeats: (scheduleId: number) => void;
}

const BusCard = ({ bus, onViewSeats }: BusCardProps) => {
  const amenityNames = getAmenityNames(bus.amenities);
  const busTypeName = getBusTypeName(bus.busType);
  
  const formatTime = (time: string) => {
    return new Date(`1970-01-01T${time}`).toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  };

  const calculateDuration = () => {
    const departure = new Date(`1970-01-01T${bus.departureTime}`);
    const arrival = new Date(`1970-01-01T${bus.arrivalTime}`);
    const diff = arrival.getTime() - departure.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    return `${hours}h ${minutes}m`;
  };

  const getAmenityIcon = (amenity: string) => {
    switch (amenity) {
      case 'WiFi': return <Wifi fontSize="small" />;
      case 'Charging': return <Power fontSize="small" />;
      case 'Snacks': return <Restaurant fontSize="small" />;
      default: return null;
    }
  };

  return (
    <Card sx={{ mb: 2, '&:hover': { boxShadow: 3 } }}>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <Box sx={{ flex: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <Typography variant="h6" sx={{ fontWeight: 600 }}>
                🚌 {bus.busName}
              </Typography>
              <Chip label={busTypeName} size="small" color="primary" />
            </Box>
            
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <Rating value={4.2} precision={0.1} size="small" readOnly />
              <Typography variant="body2" color="text.secondary">
                4.2 | {bus.availableSeats} seats available
              </Typography>
            </Box>

            <Box sx={{ display: 'flex', alignItems: 'center', gap: 3, mb: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <AccessTime fontSize="small" color="action" />
                <Typography variant="body1" sx={{ fontWeight: 500 }}>
                  {formatTime(bus.departureTime)} → {formatTime(bus.arrivalTime)}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  ({calculateDuration()})
                </Typography>
              </Box>
            </Box>

            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
              <EventSeat fontSize="small" color="action" />
              <Typography variant="body2" color="text.secondary">
                {busTypeName}
              </Typography>
              {amenityNames.slice(0, 3).map((amenity) => (
                <Box key={amenity} sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                  {getAmenityIcon(amenity)}
                  <Typography variant="body2" color="text.secondary">
                    {amenity}
                  </Typography>
                </Box>
              ))}
              {amenityNames.length > 3 && (
                <Typography variant="body2" color="text.secondary">
                  +{amenityNames.length - 3} more
                </Typography>
              )}
            </Box>
          </Box>

          <Box sx={{ textAlign: 'right', ml: 2 }}>
            <Typography variant="h5" sx={{ fontWeight: 600, color: 'primary.main' }}>
              ₹{bus.basePrice}
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              onwards
            </Typography>
            <Button
              variant="contained"
              onClick={() => onViewSeats(bus.scheduleId)}
              sx={{ minWidth: 120 }}
            >
              VIEW SEATS →
            </Button>
          </Box>
        </Box>

        <Box sx={{ mt: 2, pt: 2, borderTop: 1, borderColor: 'divider' }}>
          <Typography variant="body2" color="text.secondary">
            {bus.vendorName} • {bus.source} → {bus.destination}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
};

export default BusCard;