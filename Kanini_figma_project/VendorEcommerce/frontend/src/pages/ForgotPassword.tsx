import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../Services/authService';

const ForgotPassword: React.FC = () => {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [emailError, setEmailError] = useState('');
  
  const navigate = useNavigate();

  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return 'Email is required';
    if (!emailRegex.test(email)) return 'Invalid email format';
    return '';
  };

  const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setEmail(value);
    setEmailError(validateEmail(value));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const emailValidation = validateEmail(email);
    if (emailValidation) {
      setEmailError(emailValidation);
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await authService.forgotPassword(email);
      
      navigate('/verify-otp', {
        state: {
          email: email,
          otpToken: response.otpToken,
          type: 'forgot-password'
        }
      });
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to send reset email');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="forgot-password-container">
        <div className="forgot-password-background">
          <div className="background-shapes">
            <div className="shape shape-1"></div>
            <div className="shape shape-2"></div>
            <div className="shape shape-3"></div>
          </div>
        </div>
        
        <div className="container">
          <div className="row justify-content-center align-items-center min-vh-100">
            <div className="col-md-6 col-lg-5 col-xl-4">
              <div className="forgot-password-card">
                <div className="forgot-password-header">
                  <h2 className="forgot-password-title">Forgot Password</h2>
                  <p className="forgot-password-subtitle">
                    Enter your email address and we'll send you a code to reset your password.
                  </p>
                </div>
                
                {error && (
                  <div className="alert alert-danger modern-alert" role="alert">
                    {error}
                  </div>
                )}

                <form onSubmit={handleSubmit} className="forgot-password-form">
                  <div className="form-group">
                    <label className="form-label">
                      Email Address
                    </label>
                    <input
                      type="email"
                      className={`form-control modern-input ${emailError ? 'error' : ''}`}
                      placeholder="Enter your email"
                      value={email}
                      onChange={handleEmailChange}
                      required
                    />
                    {emailError && <div className="error-message">{emailError}</div>}
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary modern-btn w-100"
                    disabled={loading || !!emailError || !email}
                  >
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                        Sending Code...
                      </>
                    ) : (
                      'Send Reset Code'
                    )}
                  </button>
                  
                  <div className="divider">
                    <span>or</span>
                  </div>
                  
                  <button
                    type="button"
                    className="btn btn-outline-primary modern-btn-outline w-100"
                    onClick={() => navigate('/login')}
                  >
                    Back to Login
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <style>{`
        .forgot-password-container {
          min-height: 100vh;
          position: relative;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          overflow: hidden;
        }
        
        .forgot-password-background {
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
          width: 160px;
          height: 160px;
          top: 20%;
          left: 15%;
          animation-delay: 0s;
        }
        
        .shape-2 {
          width: 120px;
          height: 120px;
          top: 60%;
          right: 20%;
          animation-delay: 2s;
        }
        
        .shape-3 {
          width: 80px;
          height: 80px;
          bottom: 25%;
          left: 25%;
          animation-delay: 4s;
        }
        
        @keyframes float {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          50% { transform: translateY(-15px) rotate(180deg); }
        }
        
        .container {
          position: relative;
          z-index: 2;
        }
        
        .forgot-password-card {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border-radius: var(--radius-xl);
          padding: 2.5rem;
          box-shadow: var(--shadow-xl);
          border: 1px solid rgba(255, 255, 255, 0.2);
          transition: all 0.3s ease;
        }
        
        .forgot-password-card:hover {
          transform: translateY(-5px);
          box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
        }
        
        .forgot-password-header {
          text-align: center;
          margin-bottom: 2rem;
        }
        
        .forgot-password-title {
          font-size: 1.875rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
        }
        
        .forgot-password-subtitle {
          color: var(--gray-600);
          font-size: 1rem;
          margin: 0;
          line-height: 1.5;
        }
        
        .modern-alert {
          padding: 1rem 1.25rem;
          border-radius: var(--radius-md);
          border: none;
          background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
          color: #991b1b;
          margin-bottom: 1.5rem;
        }
        
        .forgot-password-form {
          display: flex;
          flex-direction: column;
          gap: 1.5rem;
        }
        
        .form-group {
          display: flex;
          flex-direction: column;
        }
        
        .form-label {
          font-weight: 600;
          color: var(--gray-700);
          margin-bottom: 0.75rem;
          font-size: 0.875rem;
        }
        
        .modern-input {
          border: 2px solid var(--gray-200);
          border-radius: var(--radius-md);
          padding: 0.875rem 1rem;
          font-size: 1rem;
          transition: all 0.2s ease;
          background: rgba(255, 255, 255, 0.8);
        }
        
        .modern-input:focus {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 4px rgba(99, 102, 241, 0.1);
          background: white;
          outline: none;
        }
        
        .modern-input.error {
          border-color: var(--danger-color);
        }
        
        .error-message {
          color: var(--danger-color);
          font-size: 0.75rem;
          margin-top: 0.25rem;
          font-weight: 500;
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
          cursor: pointer;
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
          cursor: pointer;
        }
        
        .modern-btn-outline:hover {
          background: var(--primary-color);
          color: white;
          transform: translateY(-2px);
        }
        
        .divider {
          position: relative;
          text-align: center;
          margin: 1rem 0;
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
        
        @media (max-width: 768px) {
          .forgot-password-card {
            padding: 2rem 1.5rem;
            margin: 1rem;
          }
          
          .forgot-password-title {
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

export default ForgotPassword;