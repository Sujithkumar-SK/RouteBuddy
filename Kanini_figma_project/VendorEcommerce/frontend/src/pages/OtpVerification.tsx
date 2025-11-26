import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../Services/authService';

interface LocationState {
  email: string;
  otpToken: string;
  role?: number;
  formData?: any;
  type: 'registration' | 'forgot-password';
}

const OtpVerification: React.FC = () => {
  const [otp, setOtp] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [resendLoading, setResendLoading] = useState(false);
  const [success, setSuccess] = useState('');
  const [timer, setTimer] = useState(180);
  const [canResend, setCanResend] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();
  const { email, otpToken, role, formData, type } = location.state as LocationState || {};

  if (!email || !otpToken || !type) {
    navigate(type === 'forgot-password' ? '/forgot-password' : '/register');
    return null;
  }

  useEffect(() => {
    if (timer > 0) {
      const interval = setInterval(() => {
        setTimer(prev => prev - 1);
      }, 1000);
      return () => clearInterval(interval);
    } else {
      setCanResend(true);
    }
  }, [timer]);

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (type === 'registration') {
        const response = await authService.verifyOtp(email, otp, otpToken, role!);
        const message = response.message || 'Registration completed successfully!';
        
        if (response.requiresApproval || response.RequiresApproval) {
          setSuccess(`${message} Your account is pending admin approval. You'll be notified once approved.`);
          setTimeout(() => {
            navigate('/login');
          }, 2000);
        } else if (response.requiresVendorProfile || response.RequiresVendorProfile){
          setSuccess(`${message} Redirecting to complete your vendor profile...`);
          setTimeout(() => {
            navigate('/vendor-profile-setup', {
              state: { userId: response.userId, email: response.email}
            });
          }, 2000);
        } else {
          setSuccess(`${message} Redirecting to login...`);
          setTimeout(() => {
            navigate('/login');
          }, 2000);
        }
      } else {
        await authService.verifyForgotPasswordOtp(email, otp, otpToken);
        setSuccess('OTP verified successfully! Redirecting to reset password...');

        setTimeout(() => {
          navigate('/reset-password', {
            state: { email: email }
          });
        }, 2000);
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.description || 'OTP verification failed';
      
      if (errorMessage.includes('expired') || errorMessage.includes('invalid')) {
        setError(errorMessage + ' You can resend OTP after the timer expires.');
      } else {
        setError(errorMessage);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleResendOtp = async () => {
    setResendLoading(true);
    setError('');
    setSuccess('');

    try {
      if (type === 'registration' && formData) {
        const response = await authService.register(formData);
        location.state.otpToken = response.otpToken;
      } else if (type === 'forgot-password') {
        const response = await authService.forgotPassword(email);
        location.state.otpToken = response.otpToken;
      }

      setSuccess('New OTP sent to your email!');
      setTimer(180);
      setCanResend(false);
      setOtp('');
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to resend OTP');
    } finally {
      setResendLoading(false);
    }
  };

  const getTitle = () => {
    return type === 'registration' ? 'Verify Registration' : 'Verify Reset Code';
  };

  const getDescription = () => {
    return type === 'registration' 
      ? 'We\'ve sent a 6-digit verification code to' 
      : 'We\'ve sent a 6-digit reset code to';
  };

  const getBackRoute = () => {
    return type === 'registration' ? '/register' : '/forgot-password';
  };

  return (
    <>
      <div className="otp-container">
        <div className="otp-background">
          <div className="background-shapes">
            <div className="shape shape-1"></div>
            <div className="shape shape-2"></div>
            <div className="shape shape-3"></div>
          </div>
        </div>
        
        <div className="container">
          <div className="row justify-content-center align-items-center min-vh-100">
            <div className="col-md-6 col-lg-5 col-xl-4">
              <div className="otp-card">
                <div className="otp-header">
                  <h2 className="otp-title">{getTitle()}</h2>
                  <p className="otp-subtitle">
                    {getDescription()} <strong>{email}</strong>
                  </p>
                </div>
                
                {error && (
                  <div className="alert alert-danger modern-alert" role="alert">
                    {error}
                  </div>
                )}

                {success && (
                  <div className="alert alert-success modern-alert" role="alert">
                    {success}
                  </div>
                )}

                <form onSubmit={handleSubmit} className="otp-form">
                  <div className="form-group">
                    <label className="form-label">
                      Enter Verification Code
                    </label>
                    <input
                      type="text"
                      className="form-control modern-input otp-input"
                      placeholder="123456"
                      value={otp}
                      onChange={(e) => setOtp(e.target.value.replace(/\D/g, ''))}
                      maxLength={6}
                      disabled={loading || success.includes('successfully')}
                      required
                    />
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary modern-btn w-100"
                    disabled={loading || otp.length !== 6 || success.includes('successfully')}
                  >
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                        Verifying...
                      </>
                    ) : (
                      'Verify Code'
                    )}
                  </button>

                  <div className="resend-section">
                    {!canResend ? (
                      <p className="timer-text">
                        Resend code in {formatTime(timer)}
                      </p>
                    ) : (
                      <button
                        type="button"
                        className="btn btn-outline-primary modern-btn-outline w-100"
                        onClick={handleResendOtp}
                        disabled={resendLoading}
                      >
                        {resendLoading ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                            Sending...
                          </>
                        ) : (
                          'Resend Code'
                        )}
                      </button>
                    )}
                  </div>

                  <div className="divider">
                    <span>or</span>
                  </div>

                  <button
                    type="button"
                    className="btn btn-outline-secondary modern-btn-outline w-100"
                    onClick={() => navigate(getBackRoute())}
                    disabled={loading || resendLoading}
                  >
                    Back to {type === 'registration' ? 'Registration' : 'Email Entry'}
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <style>{`
        .otp-container {
          min-height: 100vh;
          position: relative;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          overflow: hidden;
        }
        
        .otp-background {
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
          width: 180px;
          height: 180px;
          top: 15%;
          left: 10%;
          animation-delay: 0s;
        }
        
        .shape-2 {
          width: 120px;
          height: 120px;
          top: 65%;
          right: 15%;
          animation-delay: 2s;
        }
        
        .shape-3 {
          width: 90px;
          height: 90px;
          bottom: 20%;
          left: 20%;
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
        
        .otp-card {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border-radius: var(--radius-xl);
          padding: 2.5rem;
          box-shadow: var(--shadow-xl);
          border: 1px solid rgba(255, 255, 255, 0.2);
          transition: all 0.3s ease;
        }
        
        .otp-card:hover {
          transform: translateY(-5px);
          box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
        }
        
        .otp-header {
          text-align: center;
          margin-bottom: 2rem;
        }
        
        .otp-title {
          font-size: 1.875rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
        }
        
        .otp-subtitle {
          color: var(--gray-600);
          font-size: 1rem;
          margin: 0;
          line-height: 1.5;
        }
        
        .modern-alert {
          padding: 1rem 1.25rem;
          border-radius: var(--radius-md);
          border: none;
          margin-bottom: 1.5rem;
          font-weight: 500;
        }
        
        .alert-success {
          background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
          color: #065f46;
        }
        
        .alert-danger {
          background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
          color: #991b1b;
        }
        
        .otp-form {
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
        
        .otp-input {
          text-align: center;
          font-size: 1.5rem;
          font-weight: 600;
          letter-spacing: 0.5rem;
          font-family: monospace;
        }
        
        .modern-input:focus {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 4px rgba(99, 102, 241, 0.1);
          background: white;
          outline: none;
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
        
        .modern-btn-outline:hover:not(:disabled) {
          background: var(--primary-color);
          color: white;
          transform: translateY(-2px);
        }
        
        .resend-section {
          text-align: center;
        }
        
        .timer-text {
          color: var(--gray-600);
          font-size: 0.875rem;
          margin: 0;
          font-weight: 500;
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
          .otp-card {
            padding: 2rem 1.5rem;
            margin: 1rem;
          }
          
          .otp-title {
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

export default OtpVerification;