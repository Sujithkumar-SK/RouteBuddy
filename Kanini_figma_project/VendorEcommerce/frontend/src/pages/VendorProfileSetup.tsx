import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';

interface LocationState {
  userId: number;
  email: string;
}

interface SubscriptionPlan {
  planId: number;
  planName: string;
  maxProducts: number;
  price: number;
}

const VendorProfileSetup: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [subscriptionPlans, setSubscriptionPlans] = useState<SubscriptionPlan[]>([]);

  const [formData, setFormData] = useState({
    businessName: '',
    ownerName: '',
    businessLicenseNumber: '',
    businessAddress: '',
    city: '',
    state: '',
    pinCode: '',
    taxRegistrationNumber: '',
    subscriptionPlanId: ''
  });
  const [document, setDocument] = useState<File | null>(null);

  const navigate = useNavigate();
  const location = useLocation();
  const { userId, email } = location.state as LocationState || {};

  if (!userId || !email) {
    navigate('/register');
    return null;
  }

  useEffect(() => {
    fetchSubscriptionPlans();
  }, []);

  const fetchSubscriptionPlans = async () => {
    try {
      const response = await axios.get('http://localhost:5108/api/vendor/subscription-plans');
      const plans = Array.isArray(response.data) ? response.data : [];
      setSubscriptionPlans(plans);
    } catch (err) {
      console.error('Failed to fetch plans:', err);
      setError('Failed to load subscription plans');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (!formData.subscriptionPlanId || formData.subscriptionPlanId === '') {
      setError('Please select a subscription plan');
      setLoading(false);
      return;
    }

    if (!formData.taxRegistrationNumber || formData.taxRegistrationNumber.trim() === '') {
      setError('Tax Registration Number is required');
      setLoading(false);
      return;
    }

    const planId = parseInt(formData.subscriptionPlanId);
    if (isNaN(planId) || planId <= 0) {
      setError('Invalid subscription plan selected');
      setLoading(false);
      return;
    }

    try {
      const payload = {
        userId: userId,
        businessName: formData.businessName,
        ownerName: formData.ownerName,
        businessLicenseNumber: formData.businessLicenseNumber,
        businessAddress: formData.businessAddress,
        city: formData.city,
        state: formData.state,
        pinCode: formData.pinCode,
        taxRegistrationNumber: formData.taxRegistrationNumber,
        subscriptionPlanId: planId
      };
      
      const response = await axios.post('http://localhost:5108/api/vendor/create-profile', payload);
      
      if (document) {
        const vendorId = response.data.vendorProfile?.vendorId;
        
        if (vendorId) {
          try {
            const documentFormData = new FormData();
            documentFormData.append('document', document);
            await axios.post(`http://localhost:5108/api/vendor/${vendorId}/upload-document`, documentFormData, {
              headers: { 'Content-Type': 'multipart/form-data' }
            });
          } catch (uploadError) {
            console.error('Document upload failed:', uploadError);
          }
        }
      }

      setSuccess('Vendor profile created successfully! Your account is pending admin approval. You will be notified once approved.');
      setTimeout(() => {
        navigate('/login');
      }, 2000);
    } catch (err: any) {
      setError(err.response?.data?.description || 'Failed to create vendor profile');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="vendor-setup-container">
        <div className="vendor-setup-background">
          <div className="background-shapes">
            <div className="shape shape-1"></div>
            <div className="shape shape-2"></div>
            <div className="shape shape-3"></div>
            <div className="shape shape-4"></div>
          </div>
        </div>
        
        <div className="container">
          <div className="row justify-content-center align-items-center min-vh-100 py-4">
            <div className="col-md-10 col-lg-8 col-xl-7">
              <div className="vendor-setup-card">
                <div className="vendor-setup-header">
                  <h2 className="vendor-setup-title">Complete Your Vendor Profile</h2>
                  <p className="vendor-setup-subtitle">
                    Welcome {email}! Please complete your business information to start selling.
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

                <form onSubmit={handleSubmit} className="vendor-setup-form">
                  <div className="form-row">
                    <div className="form-group">
                      <label className="form-label">Business Name</label>
                      <input
                        type="text"
                        className="form-control modern-input"
                        placeholder="Enter your business name"
                        value={formData.businessName}
                        onChange={(e) => setFormData({ ...formData, businessName: e.target.value })}
                        required
                      />
                    </div>

                    <div className="form-group">
                      <label className="form-label">Owner Name</label>
                      <input
                        type="text"
                        className="form-control modern-input"
                        placeholder="Enter owner name"
                        value={formData.ownerName}
                        onChange={(e) => setFormData({ ...formData, ownerName: e.target.value })}
                        required
                      />
                    </div>
                  </div>

                  <div className="form-group">
                    <label className="form-label">Business License Number</label>
                    <input
                      type="text"
                      className="form-control modern-input"
                      placeholder="Enter business license number"
                      value={formData.businessLicenseNumber}
                      onChange={(e) => setFormData({ ...formData, businessLicenseNumber: e.target.value })}
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label className="form-label">Business Address</label>
                    <textarea
                      className="form-control modern-input"
                      rows={3}
                      placeholder="Enter complete business address"
                      value={formData.businessAddress}
                      onChange={(e) => setFormData({ ...formData, businessAddress: e.target.value })}
                      required
                    />
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label className="form-label">City</label>
                      <input
                        type="text"
                        className="form-control modern-input"
                        placeholder="Enter city"
                        value={formData.city}
                        onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                        required
                      />
                    </div>
                    <div className="form-group">
                      <label className="form-label">State</label>
                      <input
                        type="text"
                        className="form-control modern-input"
                        placeholder="Enter state"
                        value={formData.state}
                        onChange={(e) => setFormData({ ...formData, state: e.target.value })}
                        required
                      />
                    </div>
                    <div className="form-group">
                      <label className="form-label">Pin Code</label>
                      <input
                        type="text"
                        className="form-control modern-input"
                        placeholder="123456"
                        value={formData.pinCode}
                        onChange={(e) => setFormData({ ...formData, pinCode: e.target.value.replace(/\D/g, '').slice(0, 6) })}
                        required
                        maxLength={6}
                      />
                    </div>
                  </div>

                  <div className="form-group">
                    <label className="form-label">Tax Registration Number</label>
                    <input
                      type="text"
                      className="form-control modern-input"
                      placeholder="Enter tax registration number"
                      value={formData.taxRegistrationNumber}
                      onChange={(e) => setFormData({ ...formData, taxRegistrationNumber: e.target.value })}
                      required
                    />
                    <small className="form-help">This field is required for vendor registration</small>
                  </div>

                  <div className="form-group">
                    <label className="form-label">Business Document (Optional)</label>
                    <input
                      type="file"
                      className="form-control modern-input file-input"
                      accept=".pdf"
                      onChange={(e) => setDocument(e.target.files?.[0] || null)}
                    />
                    <small className="form-help">Upload business license or registration document (PDF only)</small>
                  </div>

                  <div className="form-group">
                    <label className="form-label">Subscription Plan</label>
                    <select
                      className="form-control modern-select"
                      value={formData.subscriptionPlanId}
                      onChange={(e) => setFormData({ ...formData, subscriptionPlanId: e.target.value })}
                      required
                    >
                      <option value="" disabled>
                        Select a subscription plan
                      </option>
                      {subscriptionPlans.map((plan, index) => (
                        <option key={`plan-${plan.planId}-${index}`} value={String(plan.planId)}>
                          {plan.planName} - ${plan.price} (Max {plan.maxProducts} products)
                        </option>
                      ))}
                    </select>
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary modern-btn w-100"
                    disabled={loading || success.includes('successfully')}
                  >
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                        Creating Profile...
                      </>
                    ) : (
                      'Complete Profile'
                    )}
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <style>{`
        .vendor-setup-container {
          min-height: 100vh;
          position: relative;
          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
          overflow: hidden;
        }
        
        .vendor-setup-background {
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
          width: 140px;
          height: 140px;
          top: 10%;
          left: 10%;
          animation-delay: 0s;
        }
        
        .shape-2 {
          width: 100px;
          height: 100px;
          top: 70%;
          right: 15%;
          animation-delay: 2s;
        }
        
        .shape-3 {
          width: 180px;
          height: 180px;
          bottom: 15%;
          left: 20%;
          animation-delay: 4s;
        }
        
        .shape-4 {
          width: 70px;
          height: 70px;
          top: 40%;
          right: 10%;
          animation-delay: 6s;
        }
        
        @keyframes float {
          0%, 100% { transform: translateY(0px) rotate(0deg); }
          33% { transform: translateY(-12px) rotate(120deg); }
          66% { transform: translateY(8px) rotate(240deg); }
        }
        
        .container {
          position: relative;
          z-index: 2;
        }
        
        .vendor-setup-card {
          background: rgba(255, 255, 255, 0.95);
          backdrop-filter: blur(20px);
          border-radius: var(--radius-xl);
          padding: 2.5rem;
          box-shadow: var(--shadow-xl);
          border: 1px solid rgba(255, 255, 255, 0.2);
          transition: all 0.3s ease;
        }
        
        .vendor-setup-card:hover {
          transform: translateY(-5px);
          box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
        }
        
        .vendor-setup-header {
          text-align: center;
          margin-bottom: 2rem;
        }
        
        .vendor-setup-title {
          font-size: 1.875rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
        }
        
        .vendor-setup-subtitle {
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
        
        .vendor-setup-form {
          display: flex;
          flex-direction: column;
          gap: 1.25rem;
        }
        
        .form-row {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
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
        
        .file-input {
          padding: 0.5rem;
        }
        
        .form-help {
          color: var(--gray-500);
          font-size: 0.75rem;
          margin-top: 0.25rem;
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
          margin-top: 1rem;
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
        
        @media (max-width: 768px) {
          .vendor-setup-card {
            padding: 2rem 1.5rem;
            margin: 1rem;
          }
          
          .vendor-setup-title {
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
          .vendor-setup-card {
            padding: 1.5rem 1rem;
          }
        }
      `}</style>
    </>
  );
};

export default VendorProfileSetup;