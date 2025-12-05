import { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  CircularProgress,
  Alert,
  Pagination,
  Menu,
  MenuItem,
  ButtonGroup
} from '@mui/material';
import { Add as AddIcon, ArrowDropDown as ArrowDropDownIcon } from '@mui/icons-material';
import { scheduleAPI, type VendorSchedule } from '../scheduleAPI';

interface ScheduleListProps {
  onCreateNew: () => void;
  onCreateBulk: () => void;
}

const ScheduleList = ({ onCreateNew, onCreateBulk }: ScheduleListProps) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleCreateSingle = () => {
    handleClose();
    onCreateNew();
  };

  const handleCreateBulk = () => {
    handleClose();
    onCreateBulk();
  };
  const [schedules, setSchedules] = useState<VendorSchedule[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 10;

  useEffect(() => {
    fetchSchedules();
  }, [page]);

  const fetchSchedules = async () => {
    try {
      setLoading(true);
      const response = await scheduleAPI.getMySchedules(page, pageSize);
      const scheduleData = response.data?.data || response.data || [];
      setSchedules(Array.isArray(scheduleData) ? scheduleData : []);
      setTotalCount(response.data?.totalCount || response.totalCount || 0);
    } catch (err: any) {
      setError(err.response?.data?.error || 'Failed to fetch schedules');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: number) => {
    switch (status) {
      case 1: return 'success'; // Scheduled
      case 2: return 'warning'; // Cancelled
      case 3: return 'info'; // Completed
      default: return 'default';
    }
  };

  const getStatusLabel = (status: number) => {
    switch (status) {
      case 1: return 'Scheduled';
      case 2: return 'Cancelled';
      case 3: return 'Completed';
      default: return 'Unknown';
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="between" alignItems="center" mb={3}>
        <Typography variant="h5">My Schedules</Typography>
        <ButtonGroup variant="contained">
          <Button
            startIcon={<AddIcon />}
            onClick={onCreateNew}
          >
            Create Schedule
          </Button>
          <Button
            size="small"
            onClick={handleClick}
          >
            <ArrowDropDownIcon />
          </Button>
        </ButtonGroup>
        <Menu
          anchorEl={anchorEl}
          open={open}
          onClose={handleClose}
        >
          <MenuItem onClick={handleCreateSingle}>Single Schedule</MenuItem>
          <MenuItem onClick={handleCreateBulk}>Bulk Schedules</MenuItem>
        </Menu>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Card>
        <CardContent>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Bus</TableCell>
                  <TableCell>Route</TableCell>
                  <TableCell>Date</TableCell>
                  <TableCell>Departure</TableCell>
                  <TableCell>Arrival</TableCell>
                  <TableCell>Available Seats</TableCell>
                  <TableCell>Status</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {schedules.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} align="center">
                      <Typography variant="body2" color="text.secondary">
                        No schedules found. Create your first schedule!
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  schedules.map((schedule) => (
                    <TableRow key={schedule.scheduleId}>
                      <TableCell>{schedule.busName}</TableCell>
                      <TableCell>{schedule.source} → {schedule.destination}</TableCell>
                      <TableCell>{new Date(schedule.travelDate).toLocaleDateString()}</TableCell>
                      <TableCell>{schedule.departureTime}</TableCell>
                      <TableCell>{schedule.arrivalTime}</TableCell>
                      <TableCell>{schedule.availableSeats}</TableCell>
                      <TableCell>
                        <Chip
                          label={getStatusLabel(schedule.status)}
                          color={getStatusColor(schedule.status)}
                          size="small"
                        />
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>

          {totalCount > pageSize && (
            <Box display="flex" justifyContent="center" mt={2}>
              <Pagination
                count={Math.ceil(totalCount / pageSize)}
                page={page}
                onChange={(_, newPage) => setPage(newPage)}
                color="primary"
              />
            </Box>
          )}
        </CardContent>
      </Card>
    </Box>
  );
};

export default ScheduleList;