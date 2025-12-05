import { Card, CardContent, Typography, Box, List, ListItem, ListItemIcon, ListItemText, Chip } from '@mui/material';
import { Notifications, BookOnline, Info } from '@mui/icons-material';

interface VendorNotification {
  type: string;
  message: string;
  time: string;
}

interface NotificationsCardProps {
  notifications: VendorNotification[];
}

const NotificationsCard = ({ notifications }: NotificationsCardProps) => {
  const getNotificationIcon = (type: string) => {
    switch (type.toLowerCase()) {
      case 'booking':
        return <BookOnline color="primary" />;
      default:
        return <Info color="info" />;
    }
  };

  const formatTime = (timeString: string) => {
    const date = new Date(timeString);
    const now = new Date();
    const diffInHours = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60));
    
    if (diffInHours < 1) return 'Just now';
    if (diffInHours < 24) return `${diffInHours}h ago`;
    return date.toLocaleDateString();
  };

  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
          <Box display="flex" alignItems="center">
            <Notifications sx={{ mr: 1, color: 'primary.main' }} />
            <Typography variant="h6">Notifications</Typography>
          </Box>
          <Chip label={notifications.length} size="small" color="primary" />
        </Box>

        {notifications.length === 0 ? (
          <Box textAlign="center" py={3}>
            <Typography variant="body2" color="text.secondary">
              No new notifications
            </Typography>
          </Box>
        ) : (
          <List dense>
            {notifications.slice(0, 5).map((notification, index) => (
              <ListItem key={index} divider={index < notifications.length - 1}>
                <ListItemIcon>
                  {getNotificationIcon(notification.type)}
                </ListItemIcon>
                <ListItemText
                  primary={notification.message}
                  secondary={formatTime(notification.time)}
                  primaryTypographyProps={{ variant: 'body2' }}
                  secondaryTypographyProps={{ variant: 'caption' }}
                />
              </ListItem>
            ))}
          </List>
        )}
      </CardContent>
    </Card>
  );
};

export default NotificationsCard;