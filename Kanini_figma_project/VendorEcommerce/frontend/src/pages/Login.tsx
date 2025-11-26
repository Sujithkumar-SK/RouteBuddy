import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import { authService } from '../Services/authService';

const Login: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    
    try {
      const response = await authService.login({ email, password });
      
      const userData = {
        userId: response.userId,
        email: response.email,
        role: response.role
      };

      login(userData, response.accessToken, response.refreshToken);
      
      switch (response.role.toLowerCase()) {
        case 'admin':
          navigate('/admin-dashboard');
          break;
        case 'vendor':
          navigate('/vendor-dashboard');
          break;
        case 'customer':
        default:
          navigate('/dashboard');
          break;
      }
    } catch (err: any) {
      console.error('Login error:', err);
      setError(err.response?.data?.description || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="login-container">
        <div className="login-background">
          <div className="background-shapes">
            <div className="shape shape-1"></div>
            <div className="shape shape-2"></div>
            <div className="shape shape-3"></div>
          </div>
        </div>
        
        <div className="container">
          <div className="row justify-content-center align-items-center min-vh-100">
            <div className="col-md-6 col-lg-5 col-xl-4">
              <div className="login-card">
                <div className="login-header">
                  <h2 className="login-title">Welcome Back</h2>
                  <p className="login-subtitle">Sign in to your account</p>
                </div>
                
                {error && (
                  <div className="alert alert-danger modern-alert" role="alert">
                    {error}
                  </div>
                )}

                <form onSubmit={handleSubmit} className="login-form">
                  <div className="form-group">
                    <label htmlFor="email" className="form-label">
                      Email Address
                    </label>
                    <input
                      type="email"
                      className="form-control modern-input"
                      id="email"
                      placeholder="Enter your email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      required
                    />
                  </div>
                  
                  <div className="form-group">
                    <label htmlFor="password" className="form-label">
                      Password
                    </label>
                    <input
                      type="password"
                      className="form-control modern-input"
                      id="password"
                      placeholder="Enter your password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      required
                    />
                  </div>
                  
                  <button
                    type="submit"
                    className="btn btn-primary modern-btn w-100"
                    disabled={loading}
                  >
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                        Signing in...
                      </>
                    ) : (
                      'Sign In'
                    )}
                  </button>
                  
                  <div className="login-links">
                    <button
                      type="button"
                      className="link-btn"
                      onClick={() => navigate('/forgot-password')}
                    >
                      Forgot Password?
                    </button>
                  </div>
                  
                  <div className="divider">
                    <span>or</span>
                  </div>
                  
                  <button
                    type="button"
                    className="btn btn-outline-primary modern-btn-outline w-100"
                    onClick={() => navigate('/register')}
                  >
                    Create New Account
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <style>{`
        .login-container {
          min-height: 100vh;
          position: relative;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          overflow: hidden;
        }
        
        .login-background {
          position: absolute;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          z-index: 1;
        }
        
        .background-shapes {
          position: relative;
          width: 100%;
          height: 100%;
        }
        
        .shape {
          position: absolute;
          border-radius: 50%;
          background: rgba(255, 255, 255, 0.1);
          backdrop-filter: blur(10px);
          animation: float 6s ease-in-out infinite;
        }
        
        .shape-1 {
          width: 200px;
          height: 200px;
          top: 10%;
          left: 10%;
          animation-delay: 0s;
        }
        
        .shape-2 {
          width: 150px;
          height: 150px;
          top: 60%;
          right: 15%;
          animation-delay: 2s;
        }
        
        .shape-3 {
          width: 100px;
          height: 100px;
          bottom: 20%;
          left: 20%;
          animation-delay: 4s;
        }
        
        @keyframes float {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          50% { transform: translateY(-20px) rotate(180deg); }
        }
        
        .container {
          position: relative;
          z-index: 2;
        }
        
        .login-card {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border-radius: var(--radius-xl);
          padding: 2.5rem;
          box-shadow: var(--shadow-xl);
          border: 1px solid rgba(255, 255, 255, 0.2);
          transition: all 0.3s ease;
        }
        
        .login-card:hover {
          transform: translateY(-5px);
          box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
        }
        
        .login-header {
          text-align: center;
          margin-bottom: 2rem;
        }
        
        .login-icon {
          font-size: 3rem;
          margin-bottom: 1rem;
          display: block;
        }
        
        .login-title {
          font-size: 1.875rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
        }
        
        .login-subtitle {
          color: var(--gray-600);
          font-size: 1rem;
          margin: 0;
        }
        
        .modern-alert {
          display: flex;
          align-items: center;
          gap: 0.75rem;
          padding: 1rem 1.25rem;
          border-radius: var(--radius-md);
          border: none;
          background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
          color: #991b1b;
          margin-bottom: 1.5rem;
        }
        
        .alert-icon {
          font-size: 1.25rem;
        }
        
        .form-group {
          margin-bottom: 1.5rem;
        }
        
        .form-label {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          font-weight: 600;
          color: var(--gray-700);
          margin-bottom: 0.75rem;
          font-size: 0.875rem;
        }
        
        .label-icon {
          font-size: 1rem;
        }
        
        .modern-input {
          border: 2px solid var(--gray-200);
          border-radius: var(--radius-md);
          padding: 0.875rem 1.125rem;
          font-size: 1rem;
          transition: all 0.2s ease;
          background: rgba(255, 255, 255, 0.8);
        }
        
        .modern-input:focus {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 4px rgba(99, 102, 241, 0.1);
          background: white;
        }
        
        .modern-btn {
          padding: 0.875rem 1.5rem;
          font-size: 1rem;
          font-weight: 600;
          border-radius: var(--radius-md);
          border: none;
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
          color: white;
          transition: all 0.2s ease;
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 0.5rem;
          margin-bottom: 1.5rem;
        }
        
        .modern-btn:hover:not(:disabled) {
          background: linear-gradient(135deg, var(--primary-dark) 0%, #3730a3 100%);
          transform: translateY(-2px);
          box-shadow: var(--shadow-lg);
        }
        
        .modern-btn:disabled {
          opacity: 0.7;
          cursor: not-allowed;
        }
        
        .modern-btn-outline {
          padding: 0.875rem 1.5rem;
          font-size: 1rem;
          font-weight: 600;
          border-radius: var(--radius-md);
          border: 2px solid var(--primary-color);
          background: transparent;
          color: var(--primary-color);
          transition: all 0.2s ease;
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 0.5rem;
        }
        
        .modern-btn-outline:hover {
          background: var(--primary-color);
          color: white;
          transform: translateY(-2px);
        }
        
        .btn-icon {
          font-size: 1rem;
        }
        
        .login-links {
          text-align: center;
          margin-bottom: 1.5rem;
        }
        
        .link-btn {
          background: none;
          border: none;
          color: var(--primary-color);
          font-weight: 500;
          text-decoration: none;
          cursor: pointer;
          transition: all 0.2s ease;
          display: inline-flex;
          align-items: center;
          gap: 0.5rem;
          font-size: 0.875rem;
        }
        
        .link-btn:hover {
          color: var(--primary-dark);
          transform: translateY(-1px);
        }
        
        .link-icon {
          font-size: 1rem;
        }
        
        .divider {
          position: relative;
          text-align: center;
          margin: 1.5rem 0;
        }
        
        .divider::before {
          content: '';
          position: absolute;
          top: 50%;
          left: 0;
          right: 0;
          height: 1px;
          background: var(--gray-300);
        }
        
        .divider span {
          background: rgba(255, 255, 255, 0.95);
          padding: 0 1rem;
          color: var(--gray-500);
          font-size: 0.875rem;
          font-weight: 500;
        }
        
        @media (max-width: 576px) {
          .login-card {
            padding: 2rem 1.5rem;
            margin: 1rem;
          }
          
          .login-title {
            font-size: 1.5rem;
          }
          
          .shape {
            display: none;
          }
        }
      `}</style>
    </>
  );
};

export default Login;