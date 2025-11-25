import { AppBar, Toolbar, Typography, Button, Box, IconButton, Menu, MenuItem, Avatar } from '@mui/material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { logout } from '../../features/auth/authSlice';
import { ROUTES } from '../../utils/constants';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import DirectionsBusIcon from '@mui/icons-material/DirectionsBus';

const Navbar = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    dispatch(logout());
    navigate(ROUTES.LOGIN);
    handleMenuClose();
  };

  return (
    <AppBar position="sticky" sx={{ bgcolor: '#d84e55' }}>
      <Toolbar>
        <DirectionsBusIcon sx={{ mr: 1, fontSize: 32 }} />
        <Typography
          variant="h5"
          sx={{ flexGrow: 0, fontWeight: 700, cursor: 'pointer', mr: 4 }}
          onClick={() => navigate(ROUTES.CUSTOMER_DASHBOARD)}
        >
          RouteBuddy
        </Typography>

        <Box sx={{ flexGrow: 1, display: 'flex', gap: 2 }}>
          <Button color="inherit" onClick={() => navigate(ROUTES.CUSTOMER_DASHBOARD)}>
            Bus Tickets
          </Button>
          <Button color="inherit">My Bookings</Button>
          <Button color="inherit">Help</Button>
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Typography variant="body2" sx={{ mr: 1 }}>
            {user?.email}
          </Typography>
          <IconButton color="inherit" onClick={handleMenuOpen}>
            <Avatar sx={{ width: 32, height: 32, bgcolor: 'white', color: '#d84e55' }}>
              <AccountCircleIcon />
            </Avatar>
          </IconButton>
          <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
            <MenuItem disabled>
              <Typography variant="body2" color="text.secondary">
                {user?.role}
              </Typography>
            </MenuItem>
            <MenuItem onClick={handleMenuClose}>My Profile</MenuItem>
            <MenuItem onClick={handleLogout}>Logout</MenuItem>
          </Menu>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
