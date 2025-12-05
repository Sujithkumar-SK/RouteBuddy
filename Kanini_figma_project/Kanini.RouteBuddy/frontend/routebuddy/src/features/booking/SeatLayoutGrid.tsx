import { Box, Typography, Paper, Grid, Chip } from '@mui/material';
import { useAppSelector, useAppDispatch } from '../../hooks/useAppDispatch';
import { toggleSeatSelection } from './bookingSlice';
import SeatButton from '../../components/ui/SeatButton';
import { SeatType, SeatPosition, PriceTier } from './types';

const SeatLayoutGrid = () => {
  const dispatch = useAppDispatch();
  const { seatLayout, selectedSeats } = useAppSelector((state) => state.booking);

  if (!seatLayout) {
    return (
      <Paper sx={{ p: 3, textAlign: 'center' }}>
        <Typography>No seat layout available</Typography>
      </Paper>
    );
  }

  const handleSeatSelect = (seatNumber: string) => {
    dispatch(toggleSeatSelection(seatNumber));
  };

  // Group seats by row for grid display
  const seatsByRow = seatLayout.seats.reduce((acc, seat) => {
    if (!acc[seat.rowNumber]) {
      acc[seat.rowNumber] = [];
    }
    acc[seat.rowNumber].push(seat);
    return acc;
  }, {} as Record<number, typeof seatLayout.seats>);

  // Sort seats within each row by column number
  Object.keys(seatsByRow).forEach(row => {
    seatsByRow[parseInt(row)].sort((a, b) => a.columnNumber - b.columnNumber);
  });

  const maxColumns = Math.max(...seatLayout.seats.map(seat => seat.columnNumber));

  return (
    <Paper sx={{ p: 3 }}>
      <Box sx={{ mb: 3 }}>
        <Typography variant="h6" sx={{ mb: 1 }}>
          Select Seats (Max 6)
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          {seatLayout.bus.busName} • {seatLayout.bus.availableSeats} seats available
        </Typography>
        
        {/* Legend */}
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: 3 }}>
          <Chip label="✅ Available" size="small" color="success" variant="outlined" />
          <Chip label="❌ Booked" size="small" color="error" variant="outlined" />
          <Chip label="🟦 Selected" size="small" color="primary" />
          <Chip label="🪟 Window" size="small" variant="outlined" />
        </Box>
      </Box>

      {/* Driver Section */}
      <Box sx={{ textAlign: 'center', mb: 2, p: 1, bgcolor: 'grey.100', borderRadius: 1 }}>
        <Typography variant="body2" sx={{ fontWeight: 600 }}>
          🚗 DRIVER
        </Typography>
      </Box>

      {/* Seat Grid */}
      <Box sx={{ 
        display: 'flex', 
        flexDirection: 'column', 
        gap: 1,
        maxWidth: maxColumns * 60,
        mx: 'auto'
      }}>
        {Object.keys(seatsByRow)
          .map(Number)
          .sort((a, b) => a - b)
          .map((rowNumber) => (
            <Box
              key={rowNumber}
              sx={{
                display: 'grid',
                gridTemplateColumns: `repeat(${maxColumns}, 1fr)`,
                gap: 1,
                justifyItems: 'center',
              }}
            >
              {Array.from({ length: maxColumns }, (_, colIndex) => {
                const seat = seatsByRow[rowNumber].find(s => s.columnNumber === colIndex + 1);
                
                if (!seat) {
                  return <Box key={colIndex} sx={{ width: 45, height: 45 }} />;
                }

                return (
                  <SeatButton
                    key={seat.seatNumber}
                    seatNumber={seat.seatNumber}
                    seatType={seat.seatType}
                    seatPosition={seat.seatPosition}
                    priceTier={seat.priceTier}
                    isBooked={seat.isBooked}
                    isSelected={selectedSeats.includes(seat.seatNumber)}
                    price={seat.price}
                    onSelect={handleSeatSelect}
                  />
                );
              })}
            </Box>
          ))}
      </Box>

      {/* Selection Summary */}
      {selectedSeats.length > 0 && (
        <Box sx={{ mt: 3, p: 2, bgcolor: 'primary.50', borderRadius: 1 }}>
          <Typography variant="subtitle2" sx={{ mb: 1 }}>
            Selected Seats: {selectedSeats.join(', ')}
          </Typography>
          <Typography variant="h6" color="primary">
            Total: ₹{selectedSeats.reduce((total, seatNumber) => {
              const seat = seatLayout.seats.find(s => s.seatNumber === seatNumber);
              return total + (seat?.price || 0);
            }, 0)}
          </Typography>
        </Box>
      )}
    </Paper>
  );
};

export default SeatLayoutGrid;