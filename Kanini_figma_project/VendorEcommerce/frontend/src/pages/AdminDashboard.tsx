import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/authContext';
import { adminService, type DashboardAnalytics, type PendingVendor } from '../Services/adminService';
import VendorApprovalModal from '../componenets/VendorApprovalModal';
import CategoryManagement from '../componenets/CategoryManagement';

const AdminDashboard: React.FC = () => {
  const { user } = useAuth();
  const [analytics, setAnalytics] = useState<DashboardAnalytics | null>(null);
  const [pendingVendors, setPendingVendors] = useState<PendingVendor[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('dashboard');
  const [selectedVendor, setSelectedVendor] = useState<PendingVendor | null>(null);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      const endDate = new Date().toISOString().split('T')[0];
      const startDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];

      const [analyticsData, vendorsData] = await Promise.all([
        adminService.getDashboardAnalytics(startDate, endDate),
        adminService.getPendingVendors()
      ]);

      setAnalytics(analyticsData);
      setPendingVendors(vendorsData);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleVendorAction = async (vendorId: number, action: 'approve' | 'reject', reason: string) => {
    try {
      if (action === 'approve') {
        await adminService.approveVendor({ vendorId, approvalReason: reason });
      } else {
        await adminService.rejectVendor({ vendorId, rejectionReason: reason });
      }
      await loadDashboardData();
    } catch (error) {
      console.error(`Error ${action}ing vendor:`, error);
    }
  };

  const handleVendorClick = (vendor: PendingVendor) => {
    setSelectedVendor(vendor);
    setShowModal(true);
  };

  const handleModalClose = () => {
    setSelectedVendor(null);
    setShowModal(false);
  };

  const handleApprove = async (vendorId: number, reason: string) => {
    await handleVendorAction(vendorId, 'approve', reason);
  };

  const handleReject = async (vendorId: number, reason: string) => {
    await handleVendorAction(vendorId, 'reject', reason);
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner">
          <div className="spinner"></div>
          <p>Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <>
      <div className="admin-dashboard">
        <div className="dashboard-header">
          <div className="container-fluid">
            <div className="header-content">
              <h1 className="dashboard-title">Admin Dashboard</h1>
              <p className="dashboard-subtitle">Manage your platform efficiently</p>
            </div>
          </div>
        </div>

        <div className="container-fluid">
          <div className="dashboard-tabs">
            <div className="tab-buttons">
              <button
                className={`tab-btn ${activeTab === 'dashboard' ? 'active' : ''}`}
                onClick={() => setActiveTab('dashboard')}
              >
                Dashboard
              </button>
              <button
                className={`tab-btn ${activeTab === 'vendors' ? 'active' : ''}`}
                onClick={() => setActiveTab('vendors')}
              >
                Vendor Management
                {pendingVendors.length > 0 && (
                  <span className="notification-badge">{pendingVendors.length}</span>
                )}
              </button>
              <button
                className={`tab-btn ${activeTab === 'categories' ? 'active' : ''}`}
                onClick={() => setActiveTab('categories')}
              >
                Category Management
              </button>
            </div>
          </div>

          {activeTab === 'dashboard' && analytics && (
            <div className="dashboard-content">
              <div className="stats-grid">
                <div className="stat-card revenue">
                  <div className="stat-content">
                    <h3>₹{analytics.totalRevenueToday.toLocaleString()}</h3>
                    <p>Today's Revenue</p>
                  </div>
                </div>
                <div className="stat-card monthly">
                  <div className="stat-content">
                    <h3>₹{analytics.totalRevenueThisMonth.toLocaleString()}</h3>
                    <p>Monthly Revenue</p>
                  </div>
                </div>
                <div className="stat-card orders">
                  <div className="stat-content">
                    <h3>{analytics.totalOrdersThisMonth}</h3>
                    <p>Total Orders</p>
                  </div>
                </div>
                <div className="stat-card vendors">
                  <div className="stat-content">
                    <h3>{analytics.activeVendors}</h3>
                    <p>Active Vendors</p>
                  </div>
                </div>
              </div>

              <div className="content-grid">
                <div className="content-card">
                  <div className="card-header">
                    <h5>Recent Activities</h5>
                  </div>
                  <div className="card-body">
                    {analytics.recentActivities.length > 0 ? (
                      <div className="activity-list">
                        {analytics.recentActivities.slice(0, 5).map((activity, index) => (
                          <div key={index} className="activity-item">
                            <div className="activity-content">
                              <strong>{activity.activityType}</strong>
                              <p>{activity.description}</p>
                              <small>by {activity.userName}</small>
                            </div>
                            <div className="activity-date">
                              {new Date(activity.createdOn).toLocaleDateString()}
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <p className="no-data">No recent activities</p>
                    )}
                  </div>
                </div>

                <div className="content-card">
                  <div className="card-header">
                    <h5>Top Selling Products</h5>
                  </div>
                  <div className="card-body">
                    {analytics.topSellingProducts.length > 0 ? (
                      <div className="product-list">
                        {analytics.topSellingProducts.slice(0, 5).map((product, index) => (
                          <div key={product.productId} className="product-item">
                            <div className="product-content">
                              <strong>{product.productName}</strong>
                              <p>Sold: {product.totalSold} units</p>
                            </div>
                            <div className="product-revenue">
                              <strong>₹{product.revenue.toLocaleString()}</strong>
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <p className="no-data">No sales data available</p>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}

          {activeTab === 'vendors' && (
            <div className="vendors-section">
              <div className="content-card">
                <div className="card-header">
                  <h5>Pending Vendor Applications</h5>
                </div>
                <div className="card-body">
                  {pendingVendors.length > 0 ? (
                    <div className="table-container">
                      <table className="modern-table">
                        <thead>
                          <tr>
                            <th>Business Name</th>
                            <th>Owner</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>License</th>
                            <th>Applied On</th>
                            <th>Actions</th>
                          </tr>
                        </thead>
                        <tbody>
                          {pendingVendors.map((vendor) => (
                            <tr key={vendor.vendorId}>
                              <td>{vendor.businessName}</td>
                              <td>{vendor.ownerName}</td>
                              <td>{vendor.email}</td>
                              <td>{vendor.phone}</td>
                              <td>{vendor.businessLicenseNumber}</td>
                              <td>{new Date(vendor.createdOn).toLocaleDateString()}</td>
                              <td>
                                <button
                                  className="btn btn-primary btn-sm"
                                  onClick={() => handleVendorClick(vendor)}
                                >
                                  Review
                                </button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  ) : (
                    <div className="empty-state">
                      <p>No pending vendor applications</p>
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}

          {activeTab === 'categories' && (
            <div className="categories-section">
              <CategoryManagement />
            </div>
          )}

          <div className="user-info-card">
            <div className="user-content">
              <h6>Welcome, {user?.email}!</h6>
              <p>Role: <span className="role-badge">{user?.role}</span></p>
              <p>User ID: {user?.userId}</p>
            </div>
          </div>
        </div>

        <VendorApprovalModal
          vendor={selectedVendor}
          isOpen={showModal}
          onClose={handleModalClose}
          onApprove={handleApprove}
          onReject={handleReject}
        />
      </div>

      <style>{`
        .admin-dashboard {
          min-height: 100vh;
          background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
        }
        
        .loading-container {
          min-height: 100vh;
          display: flex;
          align-items: center;
          justify-content: center;
          background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
        }
        
        .loading-spinner {
          text-align: center;
        }
        
        .spinner {
          width: 50px;
          height: 50px;
          border: 4px solid var(--gray-200);
          border-top: 4px solid var(--primary-color);
          border-radius: 50%;
          animation: spin 1s linear infinite;
          margin: 0 auto 1rem;
        }
        
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
        
        .dashboard-header {
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
          color: white;
          padding: 3rem 0;
          margin-bottom: 2rem;
        }
        
        .header-content {
          text-align: center;
        }
        
        .dashboard-title {
          font-size: 2.5rem;
          font-weight: 700;
          margin-bottom: 0.5rem;
        }
        
        .dashboard-subtitle {
          font-size: 1.125rem;
          opacity: 0.9;
          margin: 0;
        }
        
        .dashboard-tabs {
          margin-bottom: 2rem;
        }
        
        .tab-buttons {
          display: flex;
          gap: 1rem;
          justify-content: center;
          flex-wrap: wrap;
        }
        
        .tab-btn {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          padding: 0.875rem 1.5rem;
          border: none;
          border-radius: var(--radius-lg);
          background: white;
          color: var(--gray-600);
          font-weight: 500;
          transition: all 0.2s ease;
          box-shadow: var(--shadow-sm);
          position: relative;
        }
        
        .tab-btn:hover {
          background: var(--primary-light);
          color: var(--primary-dark);
          transform: translateY(-2px);
          box-shadow: var(--shadow-md);
        }
        
        .tab-btn.active {
          background: var(--primary-color);
          color: white;
          box-shadow: var(--shadow-md);
        }
        
        .notification-badge {
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
        
        .dashboard-content {
          padding: 0 1rem;
        }
        
        .stats-grid {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
          gap: 1.5rem;
          margin-bottom: 2rem;
        }
        
        .stat-card {
          background: white;
          border-radius: var(--radius-lg);
          padding: 2rem;
          box-shadow: var(--shadow-md);
          transition: all 0.3s ease;
          text-align: center;
        }
        
        .stat-card:hover {
          transform: translateY(-5px);
          box-shadow: var(--shadow-xl);
        }
        
        .stat-card.revenue {
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
          color: white;
        }
        
        .stat-card.monthly {
          background: linear-gradient(135deg, var(--success-color) 0%, #059669 100%);
          color: white;
        }
        
        .stat-card.orders {
          background: linear-gradient(135deg, var(--info-color) 0%, #1d4ed8 100%);
          color: white;
        }
        
        .stat-card.vendors {
          background: linear-gradient(135deg, var(--warning-color) 0%, #d97706 100%);
          color: white;
        }
        
        .stat-content h3 {
          font-size: 2rem;
          font-weight: 700;
          margin-bottom: 0.5rem;
        }
        
        .stat-content p {
          font-size: 1rem;
          opacity: 0.9;
          margin: 0;
        }
        
        .content-grid {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
          gap: 1.5rem;
          margin-bottom: 2rem;
        }
        
        .content-card {
          background: white;
          border-radius: var(--radius-lg);
          box-shadow: var(--shadow-md);
          overflow: hidden;
        }
        
        .card-header {
          background: linear-gradient(135deg, var(--gray-50) 0%, var(--gray-100) 100%);
          padding: 1.25rem 1.5rem;
          border-bottom: 1px solid var(--gray-200);
        }
        
        .card-header h5 {
          margin: 0;
          font-weight: 600;
          color: var(--gray-800);
        }
        
        .card-body {
          padding: 1.5rem;
        }
        
        .activity-list, .product-list {
          display: flex;
          flex-direction: column;
          gap: 1rem;
        }
        
        .activity-item, .product-item {
          display: flex;
          justify-content: space-between;
          align-items: flex-start;
          padding: 1rem;
          background: var(--gray-50);
          border-radius: var(--radius-md);
          transition: all 0.2s ease;
        }
        
        .activity-item:hover, .product-item:hover {
          background: var(--gray-100);
          transform: translateX(5px);
        }
        
        .activity-content, .product-content {
          flex: 1;
        }
        
        .activity-content strong, .product-content strong {
          color: var(--gray-800);
          font-weight: 600;
        }
        
        .activity-content p, .product-content p {
          margin: 0.25rem 0;
          color: var(--gray-600);
          font-size: 0.875rem;
        }
        
        .activity-content small {
          color: var(--gray-500);
          font-size: 0.75rem;
        }
        
        .activity-date, .product-revenue {
          font-size: 0.875rem;
          color: var(--gray-600);
          font-weight: 500;
        }
        
        .product-revenue strong {
          color: var(--primary-color);
          font-size: 1rem;
        }
        
        .no-data {
          text-align: center;
          color: var(--gray-500);
          font-style: italic;
          padding: 2rem;
        }
        
        .vendors-section, .categories-section {
          padding: 0 1rem;
        }
        
        .table-container {
          overflow-x: auto;
        }
        
        .modern-table {
          width: 100%;
          border-collapse: separate;
          border-spacing: 0;
          border-radius: var(--radius-md);
          overflow: hidden;
          box-shadow: var(--shadow-sm);
        }
        
        .modern-table thead th {
          background: linear-gradient(135deg, var(--gray-100) 0%, var(--gray-200) 100%);
          padding: 1rem;
          font-weight: 600;
          color: var(--gray-800);
          font-size: 0.875rem;
          text-align: left;
        }
        
        .modern-table tbody td {
          padding: 1rem;
          border-top: 1px solid var(--gray-200);
          vertical-align: middle;
        }
        
        .modern-table tbody tr:hover {
          background: var(--gray-50);
        }
        
        .empty-state {
          text-align: center;
          padding: 4rem 2rem;
          color: var(--gray-500);
        }
        
        .user-info-card {
          background: white;
          border-radius: var(--radius-lg);
          padding: 1.5rem;
          box-shadow: var(--shadow-md);
          margin: 2rem 1rem 0;
        }
        
        .user-content h6 {
          color: var(--gray-800);
          font-weight: 600;
          margin-bottom: 0.5rem;
        }
        
        .user-content p {
          color: var(--gray-600);
          margin-bottom: 0.25rem;
        }
        
        .role-badge {
          background: var(--primary-color);
          color: white;
          padding: 0.25rem 0.5rem;
          border-radius: var(--radius-sm);
          font-size: 0.75rem;
          font-weight: 500;
        }
        
        @media (max-width: 768px) {
          .dashboard-title {
            font-size: 2rem;
          }
          
          .stats-grid {
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
          }
          
          .content-grid {
            grid-template-columns: 1fr;
          }
          
          .stat-card {
            padding: 1.5rem;
          }
          
          .stat-content h3 {
            font-size: 1.5rem;
          }
        }
      `}</style>
    </>
  );
};

export default AdminDashboard;