import { Chip, Button, Rating } from '@mui/material';
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
    <div className="bus-card slide-in">
      <div className="bus-card-content">
        <div className="bus-card-left">
          <div className="bus-card-header">
            <h3 className="bus-name">🚌 {bus.busName}</h3>
            <Chip label={busTypeName} size="small" color="primary" className="bus-type-badge" />
          </div>
          
          <div className="bus-rating">
            <Rating value={4.2} precision={0.1} size="small" readOnly />
            <span style={{ fontSize: '0.875rem', color: '#666' }}>
              4.2 | {bus.availableSeats} seats available
            </span>
          </div>

          <div className="bus-timing">
            <AccessTime fontSize="small" style={{ color: '#666' }} />
            <span className="bus-timing-text">
              {formatTime(bus.departureTime)} → {formatTime(bus.arrivalTime)}
            </span>
            <span className="bus-duration">({calculateDuration()})</span>
          </div>

          <div className="bus-amenities">
            <EventSeat fontSize="small" style={{ color: '#666' }} />
            <span className="amenity-item">{busTypeName}</span>
            {amenityNames.slice(0, 3).map((amenity) => (
              <span key={amenity} className="amenity-item">
                {getAmenityIcon(amenity)}
                {amenity}
              </span>
            ))}
            {amenityNames.length > 3 && (
              <span className="amenity-item">+{amenityNames.length - 3} more</span>
            )}
          </div>
        </div>

        <div className="bus-card-right">
          <div className="bus-price">₹{bus.basePrice}</div>
          <div className="bus-price-label">onwards</div>
          <Button
            variant="contained"
            onClick={() => onViewSeats(bus.scheduleId)}
            className="view-seats-button"
          >
            VIEW SEATS →
          </Button>
        </div>
      </div>

      <div className="bus-card-footer">
        {bus.vendorName} • {bus.source} → {bus.destination}
      </div>
    </div>
  );
};

export default BusCard;