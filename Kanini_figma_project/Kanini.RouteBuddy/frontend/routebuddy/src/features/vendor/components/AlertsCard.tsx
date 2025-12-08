import { Card, CardContent, Typography, Box, List, ListItem, ListItemIcon, ListItemText, Chip, Alert } from '@mui/material';
import { Warning, Error, Info, Build, CheckCircle } from '@mui/icons-material';

interface VendorAlert {
  alertId: number;
  type: string;
  message: string;
  severity: string;
  createdAt: string;
}

interface AlertsCardProps {
  alerts: VendorAlert[];
}

const AlertsCard = ({ alerts }: AlertsCardProps) => {
  const getAlertIcon = (type: string, severity: string) => {
    switch (type.toLowerCase()) {
      case 'maintenance':
        return <Build color="warning" />;
      case 'approval':
        return <CheckCircle color="info" />;
      case 'booking':
        return <Info color="primary" />;
      default:
        return severity === 'High' ? <Error color="error" /> : <Warning color="warning" />;
    }
  };

  const getSeverityColor = (severity: string) => {
    switch (severity.toLowerCase()) {
      case 'high':
        return 'error';
      case 'medium':
        return 'warning';
      case 'low':
        return 'info';
      default:
        return 'default';
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

  const highPriorityAlerts = alerts.filter(alert => alert.severity.toLowerCase() === 'high');

  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
          <Box display="flex" alignItems="center">
            <Warning sx={{ mr: 1, color: 'warning.main' }} />
            <Typography variant="h6">Alerts</Typography>
          </Box>
          <Chip 
            label={alerts.length} 
            size="small" 
            color={highPriorityAlerts.length > 0 ? 'error' : 'default'} 
          />
        </Box>

        {alerts.length === 0 ? (
          <Box textAlign="center" py={3}>
            <Typography variant="body2" color="text.secondary">
              No active alerts
            </Typography>
          </Box>
        ) : (
          <List dense>
            {alerts.slice(0, 5).map((alert) => (
              <ListItem key={alert.alertId} divider>
                <ListItemIcon>
                  {getAlertIcon(alert.type, alert.severity)}
                </ListItemIcon>
                <ListItemText
                  primary={
                    <Box display="flex" alignItems="center" gap={1}>
                      <Typography variant="body2" sx={{ flex: 1 }}>
                        {alert.message}
                      </Typography>
                      <Chip 
                        label={alert.severity} 
                        size="small" 
                        color={getSeverityColor(alert.severity)}
                        variant="outlined"
                      />
                    </Box>
                  }
                  secondary={formatTime(alert.createdAt)}
                  secondaryTypographyProps={{ variant: 'caption' }}
                />
              </ListItem>
            ))}
          </List>
        )}

        {highPriorityAlerts.length > 0 && (
          <Alert severity="warning" sx={{ mt: 2 }}>
            {highPriorityAlerts.length} high priority alert{highPriorityAlerts.length > 1 ? 's' : ''} require{highPriorityAlerts.length === 1 ? 's' : ''} attention
          </Alert>
        )}
      </CardContent>
    </Card>
  );
};

export default AlertsCard;