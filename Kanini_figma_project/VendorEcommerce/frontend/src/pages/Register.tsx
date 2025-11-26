import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../Services/authService';

const Register: React.FC = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    firstName: '',
    middleName: '',
    lastName: '',
    role: 1
  });
  
  const [errors, setErrors] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    firstName: '',
    middleName: '',
    lastName: ''
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const navigate = useNavigate();

  const validateEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) return 'Email is required';
    if (!emailRegex.test(email)) return 'Invalid email format';
    return '';
  };

  const validateName = (name: string, fieldName: string, required = true) => {
    const nameRegex = /^[A-Za-z\s]+$/;
    if (required && !name) return `${fieldName} is required`;
    if (name && !nameRegex.test(name)) return `${fieldName} should contain only letters`;
    if (name && name.length < 2) return `${fieldName} should be at least 2 characters`;
    return '';
  };

  const validatePhone = (phone: string) => {
    const phoneRegex = /^[6-9]\d{9}$/;
    if (!phone) return 'Phone number is required';
    if (!phoneRegex.test(phone)) return 'Invalid phone number (should be 10 digits starting with 6-9)';
    return '';
  };

  const validatePassword = (password: string) => {
    if (!password) return 'Password is required';
    if (password.length < 8) return 'Password should be at least 8 characters';
    if (!/(?=.*[a-z])/.test(password)) return 'Password should contain at least one lowercase letter';
    if (!/(?=.*[A-Z])/.test(password)) return 'Password should contain at least one uppercase letter';
    if (!/(?=.*\d)/.test(password)) return 'Password should contain at least one number';
    return '';
  };

  const validateConfirmPassword = (confirmPassword: string, password: string) => {
    if (!confirmPassword) return 'Confirm password is required';
    if (confirmPassword !== password) return 'Passwords do not match';
    return '';
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    
    let filteredValue = value;
    
    if (['firstName', 'middleName', 'lastName'].includes(name)) {
      filteredValue = value.replace(/[^A-Za-z\s]/g, '');
    }
    
    if (name === 'phone') {
      filteredValue = value.replace(/\D/g, '').slice(0, 10);
    }

    setFormData({
      ...formData,
      [name]: name === 'role' ? parseInt(filteredValue) : filteredValue
    });

    let fieldError = '';
    
    switch (name) {
      case 'email':
        fieldError = validateEmail(filteredValue);
        break;
      case 'firstName':
        fieldError = validateName(filteredValue, 'First name', true);
        break;
      case 'middleName':
        fieldError = validateName(filteredValue, 'Middle name', false);
        break;
      case 'lastName':
        fieldError = validateName(filteredValue, 'Last name', true);
        break;
      case 'phone':
        fieldError = validatePhone(filteredValue);
        break;
      case 'password':
        fieldError = validatePassword(filteredValue);
        if (formData.confirmPassword) {
          setErrors(prev => ({
            ...prev,
            confirmPassword: validateConfirmPassword(formData.confirmPassword, filteredValue)
          }));
        }
        break;
      case 'confirmPassword':
        fieldError = validateConfirmPassword(filteredValue, formData.password);
        break;
    }

    setErrors({
      ...errors,
      [name]: fieldError
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const { confirmPassword, ...apiData } = formData;
      const response = await authService.register(apiData);

      navigate('/verify-otp', {
        state: {
          email: formData.email,
          otpToken: response.otpToken,
          role: formData.role,
          formData: apiData,
          type : 'registration'
        }
      });
    } catch (err: any) {
      setError(err.response?.data?.description || 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  const isFormValid = () => {
    const hasNoErrors = Object.values(errors).every(error => error === '');
    const hasAllRequiredFields = formData.firstName && formData.lastName && 
                                formData.email && formData.phone && 
                                formData.password && formData.confirmPassword;
    return hasNoErrors && hasAllRequiredFields;
  };

  return (
    <>
      <div className="register-container">
        <div className="register-background">
          <div className="background-shapes">
            <div className="shape shape-1"></div>
            <div className="shape shape-2"></div>
            <div className="shape shape-3"></div>
            <div className="shape shape-4"></div>
          </div>
        </div>
        
        <div className="container">
          <div className="row justify-content-center align-items-center min-vh-100 py-4">
            <div className="col-md-8 col-lg-6 col-xl-5">
              <div className="register-card">
                <div className="register-header">
                  <h2 className="register-title">Create Account</h2>
                  <p className="register-subtitle">Join us and start your shopping journey</p>
                </div>
                
                {error && (
                  <div className="alert alert-danger modern-alert" role="alert">
                    {error}
                  </div>
                )}

                <form onSubmit={handleSubmit} className="register-form">
                  <div className="form-row">
                    <div className="form-group">
                      <label className="form-label">
                        First Name
                      </label>
                      <input
                        type="text"
                        name="firstName"
                        className={`form-control modern-input ${errors.firstName ? 'error' : ''}`}
                        placeholder="Enter your first name"
                        value={formData.firstName}
                        onChange={handleChange}
                        required
                      />
                      {errors.firstName && <div className="error-message">{errors.firstName}</div>}
                    </div>

                    <div className="form-group">
                      <label className="form-label">
                        Last Name
                      </label>
                      <input
                        type="text"
                        name="lastName"
                        className={`form-control modern-input ${errors.lastName ? 'error' : ''}`}
                        placeholder="Enter your last name"
                        value={formData.lastName}
                        onChange={handleChange}
                        required
                      />
                      {errors.lastName && <div className="error-message">{errors.lastName}</div>}
                    </div>
                  </div>

                  <div className="form-group">
                    <label className="form-label">
                      Middle Name (Optional)
                    </label>
                    <input
                      type="text"
                      name="middleName"
                      className={`form-control modern-input ${errors.middleName ? 'error' : ''}`}
                      placeholder="Enter your middle name"
                      value={formData.middleName}
                      onChange={handleChange}
                    />
                    {errors.middleName && <div className="error-message">{errors.middleName}</div>}
                  </div>

                  <div className="form-group">
                    <label className="form-label">
                      Email Address
                    </label>
                    <input
                      type="email"
                      name="email"
                      className={`form-control modern-input ${errors.email ? 'error' : ''}`}
                      placeholder="Enter your email"
                      value={formData.email}
                      onChange={handleChange}
                      required
                    />
                    {errors.email && <div className="error-message">{errors.email}</div>}
                  </div>

                  <div className="form-group">
                    <label className="form-label">
                      Phone Number
                    </label>
                    <input
                      type="tel"
                      name="phone"
                      className={`form-control modern-input ${errors.phone ? 'error' : ''}`}
                      placeholder="9876543210"
                      value={formData.phone}
                      onChange={handleChange}
                      required
                    />
                    {errors.phone && <div className="error-message">{errors.phone}</div>}
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label className="form-label">
                        Password
                      </label>
                      <input
                        type="password"
                        name="password"
                        className={`form-control modern-input ${errors.password ? 'error' : ''}`}
                        placeholder="Create a password"
                        value={formData.password}
                        onChange={handleChange}
                        required
                      />
                      {errors.password && <div className="error-message">{errors.password}</div>}
                    </div>

                    <div className="form-group">
                      <label className="form-label">
                        Confirm Password
                      </label>
                      <input
                        type="password"
                        name="confirmPassword"
                        className={`form-control modern-input ${errors.confirmPassword ? 'error' : ''}`}
                        placeholder="Confirm your password"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        required
                      />
                      {errors.confirmPassword && <div className="error-message">{errors.confirmPassword}</div>}
                    </div>
                  </div>

                  <div className="form-group">
                    <label className="form-label">
                      Account Type
                    </label>
                    <select
                      name="role"
                      className="form-control modern-select"
                      value={formData.role}
                      onChange={handleChange}
                    >
                      <option value={1}>Customer - Shop and buy products</option>
                      <option value={2}>Vendor - Sell your products</option>
                    </select>
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary modern-btn w-100"
                    disabled={loading || !isFormValid()}
                  >
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                        Creating Account...
                      </>
                    ) : (
                      'Create Account'
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
                    Already have an account? Sign In
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <style>{`
        .register-container {
          min-height: 100vh;
          position: relative;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          overflow: hidden;
        }
        
        .register-background {
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
          animation: float 8s ease-in-out infinite;
        }
        
        .shape-1 {
          width: 150px;
          height: 150px;
          top: 15%;
          left: 10%;
          animation-delay: 0s;
        }
        
        .shape-2 {
          width: 100px;
          height: 100px;
          top: 70%;
          right: 20%;
          animation-delay: 2s;
        }
        
        .shape-3 {
          width: 200px;
          height: 200px;
          bottom: 10%;
          left: 15%;
          animation-delay: 4s;
        }
        
        .shape-4 {
          width: 80px;
          height: 80px;
          top: 30%;
          right: 10%;
          animation-delay: 6s;
        }
        
        @keyframes float {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          33% { transform: translateY(-15px) rotate(120deg); }
          66% { transform: translateY(10px) rotate(240deg); }
        }
        
        .container {
          position: relative;
          z-index: 2;
        }
        
        .register-card {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border-radius: var(--radius-xl);
          padding: 2.5rem;
          box-shadow: var(--shadow-xl);
          border: 1px solid rgba(255, 255, 255, 0.2);
          transition: all 0.3s ease;
        }
        
        .register-card:hover {
          transform: translateY(-5px);
          box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
        }
        
        .register-header {
          text-align: center;
          margin-bottom: 2rem;
        }
        
        .register-title {
          font-size: 1.875rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
        }
        
        .register-subtitle {
          color: var(--gray-600);
          font-size: 1rem;
          margin: 0;
        }
        
        .modern-alert {
          padding: 1rem 1.25rem;
          border-radius: var(--radius-md);
          border: none;
          background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
          color: #991b1b;
          margin-bottom: 1.5rem;
        }
        
        .register-form {
          display: flex;
          flex-direction: column;
          gap: 1.25rem;
        }
        
        .form-row {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 1rem;
        }
        
        .form-group {
          display: flex;
          flex-direction: column;
        }
        
        .form-label {
          font-weight: 600;
          color: var(--gray-700);
          margin-bottom: 0.5rem;
          font-size: 0.875rem;
        }
        
        .modern-input, .modern-select {
          border: 2px solid var(--gray-200);
          border-radius: var(--radius-md);
          padding: 0.875rem 1rem;
          font-size: 0.875rem;
          transition: all 0.2s ease;
          background: rgba(255, 255, 255, 0.8);
        }
        
        .modern-input:focus, .modern-select:focus {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 4px rgba(99, 102, 241, 0.1);
          background: white;
          outline: none;
        }
        
        .modern-input.error, .modern-select.error {
          border-color: var(--danger-color);
        }
        
        .error-message {
          color: var(--danger-color);
          font-size: 0.75rem;
          margin-top: 0.25rem;
          font-weight: 500;
        }
        
        .modern-select {
          cursor: pointer;
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
          .register-card {
            padding: 2rem 1.5rem;
            margin: 1rem;
          }
          
          .register-title {
            font-size: 1.5rem;
          }
          
          .form-row {
            grid-template-columns: 1fr;
          }
          
          .shape {
            display: none;
          }
        }
        
        @media (max-width: 576px) {
          .register-card {
            padding: 1.5rem 1rem;
          }
        }
      `}</style>
    </>
  );
};

export default Register;