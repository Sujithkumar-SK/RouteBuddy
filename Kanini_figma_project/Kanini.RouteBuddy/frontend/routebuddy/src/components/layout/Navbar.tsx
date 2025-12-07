import { AppBar, Button, IconButton, Menu, MenuItem, Avatar, Drawer, List, ListItem, ListItemText } from '@mui/material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { logout } from '../../features/auth/authSlice';
import { ROUTES } from '../../utils/constants';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import DirectionsBusIcon from '@mui/icons-material/DirectionsBus';
import MenuIcon from '@mui/icons-material/Menu';

const Navbar = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

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

  const getNavigationLinks = () => {
    switch (user?.role) {
      case 'Vendor':
        return [
          { label: 'Dashboard', route: ROUTES.VENDOR_DASHBOARD },
          { label: 'My Fleet', route: ROUTES.VENDOR_FLEET },
          { label: 'Schedules', route: ROUTES.VENDOR_SCHEDULES },
          { label: 'Analytics', route: ROUTES.VENDOR_ANALYTICS },
        ];
      case 'Admin':
        return [
          { label: 'Dashboard', route: ROUTES.ADMIN_DASHBOARD },
          { label: 'Vendors', route: ROUTES.ADMIN_VENDORS },
          { label: 'Buses', route: ROUTES.ADMIN_BUSES },
          { label: 'Bookings', route: ROUTES.ADMIN_BOOKINGS },
          { label: 'Routes', route: ROUTES.ADMIN_ROUTES },
          { label: 'Users', route: ROUTES.ADMIN_USERS },
        ];
      default:
        return [
          { label: 'Bus Tickets', route: ROUTES.CUSTOMER_DASHBOARD },
          { label: 'My Bookings', route: ROUTES.MY_BOOKINGS },
          { label: 'Help', route: '#' },
        ];
    }
  };

  const renderNavigationItems = () => {
    switch (user?.role) {
      case 'Vendor':
        return (
          <>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.VENDOR_DASHBOARD)}>
              Dashboard
            </Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.VENDOR_FLEET)}>My Fleet</Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.VENDOR_SCHEDULES)}>Schedules</Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.VENDOR_ANALYTICS)}>Analytics</Button>
          </>
        );
      case 'Admin':
        return (
          <>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.ADMIN_DASHBOARD)}>
              Dashboard
            </Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.ADMIN_VENDORS)}>Vendors</Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.ADMIN_BUSES)}>Buses</Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.ADMIN_BOOKINGS)}>Bookings</Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.ADMIN_ROUTES)}>Routes</Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.ADMIN_USERS)}>Users</Button>
          </>
        );
      default: // Customer
        return (
          <>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.CUSTOMER_DASHBOARD)}>
              Bus Tickets
            </Button>
            <Button className="navbar-link" onClick={() => navigate(ROUTES.MY_BOOKINGS)}>My Bookings</Button>
            <Button className="navbar-link">Help</Button>
          </>
        );
    }
  };

  return (
    <AppBar position="sticky" className="navbar">
      <div className="navbar-toolbar">
        <div className="navbar-logo" onClick={() => navigate(getDashboardRoute())}>
          <DirectionsBusIcon className="navbar-logo-icon" />
          <span className="navbar-logo-text">RouteBuddy</span>
        </div>

        <div className="navbar-nav">
          {renderNavigationItems()}
        </div>

        <IconButton 
          className="navbar-mobile-toggle"
          onClick={() => setMobileMenuOpen(true)}
          sx={{ display: { xs: 'block', md: 'none' }, color: 'white' }}
        >
          <MenuIcon />
        </IconButton>

        <div className="navbar-user">
          <span className="navbar-user-email">{user?.email}</span>
          <IconButton className="navbar-avatar-button" onClick={handleMenuOpen}>
            <Avatar className="navbar-avatar">
              <AccountCircleIcon />
            </Avatar>
          </IconButton>
          <Menu 
            anchorEl={anchorEl} 
            open={Boolean(anchorEl)} 
            onClose={handleMenuClose}
            className="navbar-menu"
          >
            <MenuItem disabled className="navbar-menu-role">
              {user?.role}
            </MenuItem>
            {user?.role !== 'Admin' && (
              <MenuItem 
                onClick={() => { navigate(ROUTES.PROFILE); handleMenuClose(); }}
                className="navbar-menu-item"
              >
                My Profile
              </MenuItem>
            )}
            <MenuItem onClick={handleLogout} className="navbar-menu-item">
              Logout
            </MenuItem>
          </Menu>
        </div>
      </div>

      <Drawer
        anchor="left"
        open={mobileMenuOpen}
        onClose={() => setMobileMenuOpen(false)}
      >
        <div style={{ width: 250, padding: '1rem', background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', height: '100%' }}>
          <div style={{ color: 'white', fontSize: '1.5rem', fontWeight: 700, marginBottom: '2rem', textAlign: 'center' }}>
            RouteBuddy
          </div>
          <List>
            {getNavigationLinks().map((link) => (
              <ListItem 
                key={link.label}
                onClick={() => {
                  navigate(link.route);
                  setMobileMenuOpen(false);
                }}
                sx={{ 
                  color: 'white', 
                  cursor: 'pointer',
                  borderRadius: '8px',
                  mb: 1,
                  '&:hover': { background: 'rgba(255,255,255,0.15)' }
                }}
              >
                <ListItemText primary={link.label} />
              </ListItem>
            ))}
          </List>
        </div>
      </Drawer>
    </AppBar>
  );
};

export default Navbar;
