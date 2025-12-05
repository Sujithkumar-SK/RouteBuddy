import { Card, CardContent, Typography, Box, LinearProgress } from '@mui/material';
import { DirectionsBus, Build, PlayArrow, Pause } from '@mui/icons-material';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';
import type { VendorFleetStatus } from '../vendorAPI';

interface FleetStatusCardProps extends VendorFleetStatus {}

const FleetStatusCard = ({ totalBuses, activeBuses, maintenanceBuses, idleBuses }: FleetStatusCardProps) => {
  const getPercentage = (value: number) => totalBuses > 0 ? (value / totalBuses) * 100 : 0;

  const fleetData = [
    { name: 'Active', value: activeBuses },
    { name: 'Maintenance', value: maintenanceBuses },
    { name: 'Idle', value: idleBuses }
  ];

  const COLORS = ['#4caf50', '#ff9800', '#9e9e9e'];

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

        <Box display="grid" gridTemplateColumns="repeat(3, 1fr)" gap={2} mb={3}>
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

        <Box height={200}>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <defs>
                <linearGradient id="activeGradient" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0%" stopColor="#4caf50" stopOpacity={0.8}/>
                  <stop offset="100%" stopColor="#81c784" stopOpacity={0.6}/>
                </linearGradient>
                <linearGradient id="maintenanceGradient" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0%" stopColor="#ff9800" stopOpacity={0.8}/>
                  <stop offset="100%" stopColor="#ffb74d" stopOpacity={0.6}/>
                </linearGradient>
                <linearGradient id="idleGradient" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0%" stopColor="#9e9e9e" stopOpacity={0.8}/>
                  <stop offset="100%" stopColor="#bdbdbd" stopOpacity={0.6}/>
                </linearGradient>
              </defs>
              <Pie
                data={fleetData}
                cx="50%"
                cy="50%"
                innerRadius={40}
                outerRadius={80}
                dataKey="value"
              >
                {fleetData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={`url(#${index === 0 ? 'activeGradient' : index === 1 ? 'maintenanceGradient' : 'idleGradient'})`} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </Box>
      </CardContent>
    </Card>
  );
};

export default FleetStatusCard;