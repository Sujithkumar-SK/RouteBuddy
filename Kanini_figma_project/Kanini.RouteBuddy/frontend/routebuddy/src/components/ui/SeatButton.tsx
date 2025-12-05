import { Button, Tooltip } from '@mui/material';
import { SeatType, SeatPosition, PriceTier } from '../../features/booking/types';

interface SeatButtonProps {
  seatNumber: string;
  seatType: SeatType;
  seatPosition: SeatPosition;
  priceTier: PriceTier;
  isBooked: boolean;
  isSelected: boolean;
  price: number;
  onSelect: (seatNumber: string) => void;
}

const SeatButton = ({
  seatNumber,
  seatType,
  seatPosition,
  priceTier,
  isBooked,
  isSelected,
  price,
  onSelect,
}: SeatButtonProps) => {
  const getSeatIcon = () => {
    switch (seatType) {
      case SeatType.Seater:
      case SeatType.SingleSeater:
        return '🪑';
      case SeatType.SleeperLower:
        return '🛏️';
      case SeatType.SleeperUpper:
        return '🛏️';
      case SeatType.SemiSleeper:
      case SeatType.SingleSemiSeater:
        return '💺';
      default:
        return '💺';
    }
  };

  const getPositionIndicator = () => {
    switch (seatPosition) {
      case SeatPosition.Window:
        return '🪟';
      case SeatPosition.Aisle:
        return '';
      case SeatPosition.Middle:
        return '';
      default:
        return '';
    }
  };

  const getSeatColor = () => {
    if (isBooked) return 'error';
    if (isSelected) return 'primary';
    switch (priceTier) {
      case PriceTier.Premium:
        return 'warning';
      case PriceTier.Luxury:
        return 'secondary';
      default:
        return 'success';
    }
  };

  const getSeatVariant = () => {
    if (isBooked) return 'outlined';
    if (isSelected) return 'contained';
    return 'outlined';
  };

  const tooltipTitle = `
    Seat: ${seatNumber}
    Type: ${SeatType[seatType]}
    Position: ${SeatPosition[seatPosition]}
    Price: ₹${price}
    ${isBooked ? 'BOOKED' : 'Available'}
  `;

  return (
    <Tooltip title={tooltipTitle} arrow>
      <span>
        <Button
          variant={getSeatVariant()}
          color={getSeatColor()}
          disabled={isBooked}
          onClick={() => onSelect(seatNumber)}
          sx={{
            minWidth: 45,
            height: 45,
            fontSize: '10px',
            flexDirection: 'column',
            p: 0.5,
            border: 1,
            position: 'relative',
          }}
        >
          <div style={{ fontSize: '12px' }}>{getSeatIcon()}</div>
          <div style={{ fontSize: '8px', fontWeight: 'bold' }}>{seatNumber}</div>
          {seatPosition === SeatPosition.Window && (
            <div style={{ position: 'absolute', top: -2, right: -2, fontSize: '8px' }}>
              {getPositionIndicator()}
            </div>
          )}
        </Button>
      </span>
    </Tooltip>
  );
};

export default SeatButton;