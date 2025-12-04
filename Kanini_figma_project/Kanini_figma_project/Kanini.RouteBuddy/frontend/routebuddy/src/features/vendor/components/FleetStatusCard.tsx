import { Card, CardContent, Typography, Box, LinearProgress } from '@mui/material';
import { DirectionsBus, Build, PlayArrow, Pause } from '@mui/icons-material';
import type { VendorFleetStatus } from '../vendorAPI';

interface FleetStatusCardProps extends VendorFleetStatus {}

const FleetStatusCard = ({ totalBuses, activeBuses, maintenanceBuses, idleBuses }: FleetStatusCardProps) => {
  const getPercentage = (value: number) => totalBuses > 0 ? (value / totalBuses) * 100 : 0;

  return (
    <Card>
      <CardContent>
        <Box display="flex" alignItems="center" mb={2}>
          <DirectionsBus sx={{ mr: 1, color: 'primary.main' }} />
          <Typography variant="h6">Fleet Status</Typography>
        </Box>

        <Box mb={3}>
          <Box display="flex" justifyContent="space-between" mb={1}>
            <Typography variant="body2">Active Buses</Typography>
            <Typography variant="body2" color="success.main">
              {activeBuses}/{totalBuses}
            </Typography>
          </Box>
          <LinearProgress 
            variant="determinate" 
            value={getPercentage(activeBuses)} 
            color="success"
            sx={{ height: 8, borderRadius: 4 }}
          />
        </Box>

        <Box display="grid" gridTemplateColumns="repeat(3, 1fr)" gap={2}>
          <Box textAlign="center" p={1}>
            <PlayArrow sx={{ color: 'success.main', mb: 0.5 }} />
            <Typography variant="h6" color="success.main">{activeBuses}</Typography>
            <Typography variant="caption" color="text.secondary">Active</Typography>
          </Box>

          <Box textAlign="center" p={1}>
            <Build sx={{ color: 'warning.main', mb: 0.5 }} />
            <Typography variant="h6" color="warning.main">{maintenanceBuses}</Typography>
            <Typography variant="caption" color="text.secondary">Maintenance</Typography>
          </Box>

          <Box textAlign="center" p={1}>
            <Pause sx={{ color: 'text.secondary', mb: 0.5 }} />
            <Typography variant="h6" color="text.secondary">{idleBuses}</Typography>
            <Typography variant="caption" color="text.secondary">Idle</Typography>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export default FleetStatusCard;