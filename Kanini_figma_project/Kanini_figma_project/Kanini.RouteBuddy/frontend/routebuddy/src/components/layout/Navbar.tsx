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

  const getDashboardRoute = () => {
    switch (user?.role) {
      case 'Vendor':
        return ROUTES.VENDOR_DASHBOARD;
      case 'Admin':
        return ROUTES.ADMIN_DASHBOARD;
      default:
        return ROUTES.CUSTOMER_DASHBOARD;
    }
  };

  const renderNavigationItems = () => {
    switch (user?.role) {
      case 'Vendor':
        return (
          <>
            <Button color="inherit" onClick={() => navigate(ROUTES.VENDOR_DASHBOARD)}>
              Dashboard
            </Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.VENDOR_FLEET)}>Fleet Management</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.VENDOR_ANALYTICS)}>Analytics</Button>
          </>
        );
      case 'Admin':
        return (
          <>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_DASHBOARD)}>
              Dashboard
            </Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_VENDORS)}>Vendors</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_BUSES)}>Buses</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_BOOKINGS)}>Bookings</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_ROUTES)}>Routes</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_USERS)}>Users</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_REPORTS)}>Reports</Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.ADMIN_SETTINGS)}>Settings</Button>
          </>
        );
      default: // Customer
        return (
          <>
            <Button color="inherit" onClick={() => navigate(ROUTES.CUSTOMER_DASHBOARD)}>
              Bus Tickets
            </Button>
            <Button color="inherit" onClick={() => navigate(ROUTES.MY_BOOKINGS)}>My Bookings</Button>
            <Button color="inherit">Help</Button>
          </>
        );
    }
  };

  return (
    <AppBar position="sticky" sx={{ bgcolor: '#d84e55' }}>
      <Toolbar>
        <DirectionsBusIcon sx={{ mr: 1, fontSize: 32 }} />
        <Typography
          variant="h5"
          sx={{ flexGrow: 0, fontWeight: 700, cursor: 'pointer', mr: 4 }}
          onClick={() => navigate(getDashboardRoute())}
        >
          RouteBuddy
        </Typography>

        <Box sx={{ flexGrow: 1, display: 'flex', gap: 2 }}>
          {renderNavigationItems()}
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Typography variant="body2" sx={{ mr: 1 }}>
            {user?.email}
          </Typography>
          <IconButton color="inherit" onClick={handleMenuOpen}>
            <Avatar 
              sx={{ width: 32, height: 32, bgcolor: 'white', color: '#d84e55' }}
              // src={user?.profileImage} // Will be implemented when backend supports images
            >
              <AccountCircleIcon />
            </Avatar>
          </IconButton>
          <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
            <MenuItem disabled>
              <Typography variant="body2" color="text.secondary">
                {user?.role}
              </Typography>
            </MenuItem>
            <MenuItem onClick={() => { navigate(ROUTES.PROFILE); handleMenuClose(); }}>My Profile</MenuItem>
            <MenuItem onClick={handleLogout}>Logout</MenuItem>
          </Menu>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
