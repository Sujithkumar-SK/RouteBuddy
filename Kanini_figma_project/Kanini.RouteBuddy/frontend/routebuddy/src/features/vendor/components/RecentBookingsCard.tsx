import { Card, CardContent, Typography, Box, List, ListItem, ListItemText, Chip, Divider } from '@mui/material';
import { BookOnline, Person, Route as RouteIcon } from '@mui/icons-material';
import type { RecentBooking } from '../vendorAPI';

interface RecentBookingsCardProps {
  bookings: RecentBooking[];
}

const RecentBookingsCard = ({ bookings }: RecentBookingsCardProps) => {
  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'confirmed': return 'success';
      case 'pending': return 'warning';
      case 'cancelled': return 'error';
      default: return 'default';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" mb={2}>
          <BookOnline sx={{ mr: 1, color: 'primary.main' }} />
          <Typography variant="h6">Recent Bookings</Typography>
        </Box>

        {bookings.length === 0 ? (
          <Box textAlign="center" py={4}>
            <Typography variant="body2" color="text.secondary">
              No recent bookings found
            </Typography>
          </Box>
        ) : (
          <List sx={{ p: 0 }}>
            {bookings.map((booking, index) => (
              <Box key={booking.bookingId}>
                <ListItem sx={{ px: 0, py: 1 }}>
                  <ListItemText
                    primary={
                      <Box display="flex" justifyContent="space-between" alignItems="center">
                        <Box display="flex" alignItems="center">
                          <Person sx={{ fontSize: 16, mr: 0.5, color: 'text.secondary' }} />
                          <Typography variant="body2" fontWeight="medium">
                            {booking.customerName}
                          </Typography>
                        </Box>
                        <Chip 
                          label={booking.status} 
                          color={getStatusColor(booking.status)}
                          size="small"
                        />
                      </Box>
                    }
                    secondary={
                      <Box mt={0.5}>
                        <Box display="flex" alignItems="center" mb={0.5}>
                          <RouteIcon sx={{ fontSize: 14, mr: 0.5, color: 'text.secondary' }} />
                          <Typography variant="caption" color="text.secondary">
                            {booking.route}
                          </Typography>
                        </Box>
                        <Typography variant="caption" color="text.secondary">
                          {formatDate(booking.bookingDate)}
                        </Typography>
                      </Box>
                    }
                  />
                </ListItem>
                {index < bookings.length - 1 && <Divider />}
              </Box>
            ))}
          </List>
        )}
      </CardContent>
    </Card>
  );
};

export default RecentBookingsCard;