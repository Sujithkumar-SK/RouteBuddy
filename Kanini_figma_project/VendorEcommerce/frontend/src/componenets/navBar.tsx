import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import { cartService } from '../Services/cartService';

const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();
  const [cartItemCount, setCartItemCount] = useState(0);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  useEffect(() => {
    if (isAuthenticated && user?.role === 'Customer') {
      loadCartCount();
    }
  }, [isAuthenticated, user]);

  useEffect(() => {
    const handleCartUpdate = () => {
      if (isAuthenticated && user?.role === 'Customer') {
        loadCartCount();
      }
    };

    window.addEventListener('cartUpdated', handleCartUpdate);
    return () => window.removeEventListener('cartUpdated', handleCartUpdate);
  }, [isAuthenticated, user]);

  const loadCartCount = async () => {
    try {
      const cart = await cartService.getCart();
      setCartItemCount(cart.totalItems);
    } catch (error) {
      console.error('Error loading cart count:', error);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const toggleMobileMenu = () => {
    setMobileMenuOpen(!mobileMenuOpen);
  };

  return (
    <>
      <nav className="modern-navbar">
        <div className="container-fluid">
          <div className="navbar-content">
            <button 
              className="navbar-brand"
              onClick={() => navigate('/')}
            >
              <span className="brand-text">ShopHub</span>
            </button>
            
            <button 
              className="mobile-menu-toggle"
              type="button" 
              onClick={toggleMobileMenu}
            >
              <span></span>
              <span></span>
              <span></span>
            </button>
            
            <div className={`navbar-collapse ${mobileMenuOpen ? 'show' : ''}`}>
              <div className="navbar-nav">
                {isAuthenticated ? (
                  <>
                    {user?.role === 'Customer' && (
                      <>
                        <button 
                          className="nav-btn nav-btn-home"
                          onClick={() => navigate('/customer-dashboard')}
                        >
                          Home
                        </button>
                        <button 
                          className="nav-btn nav-btn-cart"
                          onClick={() => navigate('/cart')}
                        >
                          Cart
                          {cartItemCount > 0 && (
                            <span className="cart-badge">
                              {cartItemCount}
                            </span>
                          )}
                        </button>
                      </>
                    )}
                    
                    <div className="user-section">
                      <div className="user-welcome">
                        <div className="user-avatar">
                          {user?.email?.charAt(0).toUpperCase()}
                        </div>
                        <span className="user-name">
                          {user?.email}
                        </span>
                      </div>
                      <button className="nav-btn nav-btn-logout" onClick={handleLogout}>
                        Logout
                      </button>
                    </div>
                  </>
                ) : (
                  <div className="auth-buttons">
                    <button className="nav-btn nav-btn-login" onClick={() => navigate('/login')}>
                      Login
                    </button>
                    <button className="nav-btn nav-btn-register" onClick={() => navigate('/register')}>
                      Register
                    </button>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </nav>
      
      <style>{`
        .modern-navbar {
          background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
          backdrop-filter: blur(10px);
          border-bottom: 1px solid var(--gray-200);
          box-shadow: var(--shadow-sm);
          position: sticky;
          top: 0;
          z-index: 1000;
          padding: 0.75rem 0;
        }
        
        .navbar-content {
          display: flex;
          align-items: center;
          justify-content: space-between;
          padding: 0 1rem;
        }
        
        .navbar-brand {
          display: flex;
          align-items: center;
          gap: 0.75rem;
          background: none;
          border: none;
          font-size: 1.5rem;
          font-weight: 700;
          color: var(--gray-800);
          text-decoration: none;
          transition: all 0.2s ease;
        }
        
        .navbar-brand:hover {
          transform: scale(1.05);
        }
        
        .brand-text {
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--secondary-color) 100%);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;
          background-clip: text;
        }
        
        .mobile-menu-toggle {
          display: none;
          flex-direction: column;
          gap: 4px;
          background: none;
          border: none;
          padding: 8px;
          cursor: pointer;
        }
        
        .mobile-menu-toggle span {
          width: 25px;
          height: 3px;
          background: var(--gray-600);
          border-radius: 2px;
          transition: all 0.3s ease;
        }
        
        .navbar-collapse {
          display: flex;
        }
        
        .navbar-nav {
          display: flex;
          align-items: center;
          gap: 1rem;
          margin-left: auto;
          flex-wrap: nowrap;
        }
        
        .user-section {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          flex-wrap: nowrap;
        }
        
        .auth-buttons {
          display: flex;
          align-items: center;
          gap: 0.75rem;
          flex-wrap: nowrap;
        }
        
        .user-welcome {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          padding: 0.4rem 0.6rem;
          background: var(--gray-50);
          border-radius: var(--radius-md);
        }
        
        .user-avatar {
          width: 28px;
          height: 28px;
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--secondary-color) 100%);
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
          color: white;
          font-weight: 600;
          font-size: 0.75rem;
        }
        
        .user-name {
          font-weight: 500;
          color: var(--gray-700);
          font-size: 0.75rem;
          line-height: 1;
        }
        
        .nav-btn {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          padding: 0.5rem 1rem;
          border: none;
          border-radius: var(--radius-md);
          font-weight: 500;
          font-size: 0.875rem;
          transition: all 0.2s ease;
          position: relative;
          text-decoration: none;
          cursor: pointer;
        }
        
        .nav-btn-home {
          background: var(--gray-100);
          color: var(--gray-700);
        }
        
        .nav-btn-home:hover {
          background: var(--primary-light);
          color: var(--primary-dark);
          transform: translateY(-1px);
        }
        
        .nav-btn-cart {
          background: var(--warning-color);
          color: white;
        }
        
        .nav-btn-cart:hover {
          background: #d97706;
          transform: translateY(-1px);
        }
        
        .cart-badge {
          position: absolute;
          top: -8px;
          right: -8px;
          background: var(--danger-color);
          color: white;
          border-radius: 50%;
          width: 20px;
          height: 20px;
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 0.75rem;
          font-weight: 600;
        }
        
        .nav-btn-logout {
          background: var(--gray-100);
          color: var(--gray-700);
        }
        
        .nav-btn-logout:hover {
          background: var(--danger-color);
          color: white;
          transform: translateY(-1px);
        }
        
        .nav-btn-login {
          background: transparent;
          color: var(--primary-color);
          border: 2px solid var(--primary-color);
        }
        
        .nav-btn-login:hover {
          background: var(--primary-color);
          color: white;
          transform: translateY(-1px);
        }
        
        .nav-btn-register {
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
          color: white;
        }
        
        .nav-btn-register:hover {
          background: linear-gradient(135deg, var(--primary-dark) 0%, #3730a3 100%);
          transform: translateY(-1px);
          box-shadow: var(--shadow-md);
        }
        
        @media (max-width: 767.98px) {
          .mobile-menu-toggle {
            display: flex;
          }
          
          .navbar-collapse {
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border-top: 1px solid var(--gray-200);
            box-shadow: var(--shadow-lg);
            border-radius: 0 0 var(--radius-lg) var(--radius-lg);
            padding: 1rem;
            display: none;
          }
          
          .navbar-collapse.show {
            display: block;
          }
          
          .navbar-nav {
            flex-direction: column;
            align-items: stretch;
            gap: 0.75rem;
            margin-left: 0;
          }
          
          .user-section {
            flex-direction: column;
            align-items: stretch;
            gap: 0.5rem;
          }
          
          .auth-buttons {
            flex-direction: column;
            gap: 0.5rem;
          }
          
          .user-welcome {
            margin-bottom: 0;
          }
          
          .nav-btn {
            justify-content: center;
            width: 100%;
          }
        }
        
        @media (min-width: 768px) {
          .mobile-menu-toggle {
            display: none;
          }
          
          .navbar-collapse {
            display: flex !important;
            position: static;
            background: transparent;
            border: none;
            box-shadow: none;
            padding: 0;
          }
          
          .navbar-nav {
            flex-direction: row;
            align-items: center;
            gap: 1rem;
            margin-left: auto;
          }
          
          .user-section {
            flex-direction: row;
            align-items: center;
            gap: 0.5rem;
          }
          
          .auth-buttons {
            flex-direction: row;
            gap: 0.75rem;
          }
        }
      `}</style>
    </>
  );
};

export default Navbar;