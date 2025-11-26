import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { productService } from '../Services/productService';
import { cartService } from '../Services/cartService';
import OrderHistory from './OrderHistory';
import CustomerProfileManagement from './CustomerProfileManagement';
import type { Product, Category } from '../types/product';

const CustomerDashboard: React.FC = () => {
  const navigate = useNavigate();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [filteredProducts, setFilteredProducts] = useState<Product[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [priceRange, setPriceRange] = useState({ min: '', max: '' });
  const [sortBy, setSortBy] = useState('featured');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [addingToCart, setAddingToCart] = useState<Set<number>>(new Set());
  const [activeTab, setActiveTab] = useState('products');

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    filterProducts();
  }, [products, selectedCategory, searchTerm, priceRange, sortBy]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [productsData, categoriesData] = await Promise.all([
        productService.getAllProducts(),
        productService.getCategories()
      ]);
      setProducts(productsData);
      setCategories(categoriesData);
    } catch (err: any) {
      setError('Failed to load products');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const filterProducts = () => {
    let filtered = [...products];

    if (selectedCategory) {
      const selectedCategoryObj = categories.find(c => c.categoryId === selectedCategory);
      const categoryIds = [selectedCategory];
      
      if (selectedCategoryObj && !selectedCategoryObj.parentCategoryId) {
        const subcategories = categories.filter(c => c.parentCategoryId === selectedCategory);
        categoryIds.push(...subcategories.map(c => c.categoryId));
      }
      
      filtered = filtered.filter(product => categoryIds.includes(product.categoryId));
    }

    if (searchTerm) {
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        product.description?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (priceRange.min) {
      filtered = filtered.filter(product => product.price >= parseFloat(priceRange.min));
    }
    if (priceRange.max) {
      filtered = filtered.filter(product => product.price <= parseFloat(priceRange.max));
    }

    const sorted = [...filtered].sort((a, b) => {
      switch (sortBy) {
        case 'price-low':
          return a.price - b.price;
        case 'price-high':
          return b.price - a.price;
        case 'name':
          return a.name.localeCompare(b.name);
        case 'newest':
          return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
        default:
          return 0;
      }
    });

    setFilteredProducts(sorted);
  };

  const handleCategoryFilter = (categoryId: number | null) => {
    setSelectedCategory(categoryId);
  };

  const clearFilters = () => {
    setSelectedCategory(null);
    setSearchTerm('');
    setPriceRange({ min: '', max: '' });
    setSortBy('featured');
  };

  const handleAddToCart = async (productId: number) => {
    try {
      setAddingToCart(prev => new Set(prev).add(productId));
      await cartService.addToCart({ productId, quantity: 1 });
      
      window.dispatchEvent(new CustomEvent('cartUpdated'));
      
      const alertDiv = document.createElement('div');
      alertDiv.className = 'alert alert-success alert-dismissible fade show position-fixed';
      alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
      alertDiv.innerHTML = `
        <strong>Success!</strong> Item added to cart.
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      `;
      document.body.appendChild(alertDiv);
      
      setTimeout(() => {
        if (alertDiv.parentNode) {
          alertDiv.parentNode.removeChild(alertDiv);
        }
      }, 3000);
      
    } catch (error: any) {
      console.error('Error adding to cart:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to add item to cart';
      
      const alertDiv = document.createElement('div');
      alertDiv.className = 'alert alert-warning alert-dismissible fade show position-fixed';
      alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
      alertDiv.innerHTML = `
        <strong>Info!</strong> ${errorMsg}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      `;
      document.body.appendChild(alertDiv);
      
      setTimeout(() => {
        if (alertDiv.parentNode) {
          alertDiv.parentNode.removeChild(alertDiv);
        }
      }, 5000);
    } finally {
      setAddingToCart(prev => {
        const newSet = new Set(prev);
        newSet.delete(productId);
        return newSet;
      });
    }
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner">
          <div className="spinner"></div>
          <p>Loading amazing products...</p>
        </div>
      </div>
    );
  }

  return (
    <>
      <div className="dashboard-container">
        {error && (
          <div className="alert alert-danger modern-alert" role="alert">
            {error}
          </div>
        )}

        <div className="dashboard-header">
          <div className="container-fluid">
            <div className="header-content">
              <div className="welcome-section">
                <h1 className="dashboard-title">
                  Discover Amazing Products
                </h1>
                <p className="dashboard-subtitle">Find everything you need in one place</p>
              </div>
            </div>
          </div>
        </div>

        <div className="container-fluid">
          <div className="dashboard-tabs">
            <div className="tab-buttons">
              <button 
                className={`tab-btn ${activeTab === 'products' ? 'active' : ''}`}
                onClick={() => setActiveTab('products')}
              >
                Products
              </button>
              <button 
                className={`tab-btn ${activeTab === 'orders' ? 'active' : ''}`}
                onClick={() => setActiveTab('orders')}
              >
                My Orders
              </button>
              <button 
                className={`tab-btn ${activeTab === 'profile' ? 'active' : ''}`}
                onClick={() => setActiveTab('profile')}
              >
                Profile
              </button>
            </div>
          </div>

          {activeTab === 'orders' ? (
            <OrderHistory />
          ) : activeTab === 'profile' ? (
            <CustomerProfileManagement />
          ) : (
            <div className="products-section">
              <div className="row g-4">
                <div className="col-lg-3">
                  <div className="filters-card">
                    <div className="filters-header">
                      <h5>Filters</h5>
                      <button className="clear-filters-btn" onClick={clearFilters}>
                        Clear All
                      </button>
                    </div>
                    
                    <div className="filter-group">
                      <label className="filter-label">
                        Search Products
                      </label>
                      <input
                        type="text"
                        className="filter-input"
                        placeholder="Search products..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                      />
                    </div>

                    <div className="filter-group">
                      <label className="filter-label">
                        Categories
                      </label>
                      <div className="category-list">
                        <button
                          className={`category-item ${!selectedCategory ? 'active' : ''}`}
                          onClick={() => handleCategoryFilter(null)}
                        >
                          All Categories
                        </button>
                        {categories.map(category => (
                          <button
                            key={category.categoryId}
                            className={`category-item ${selectedCategory === category.categoryId ? 'active' : ''}`}
                            onClick={() => handleCategoryFilter(category.categoryId)}
                          >
                            {category.name}
                          </button>
                        ))}
                      </div>
                    </div>

                    <div className="filter-group">
                      <label className="filter-label">
                        Price Range
                      </label>
                      <div className="price-inputs">
                        <input
                          type="number"
                          className="filter-input"
                          placeholder="Min"
                          value={priceRange.min}
                          onChange={(e) => setPriceRange(prev => ({ ...prev, min: e.target.value }))}
                        />
                        <input
                          type="number"
                          className="filter-input"
                          placeholder="Max"
                          value={priceRange.max}
                          onChange={(e) => setPriceRange(prev => ({ ...prev, max: e.target.value }))}
                        />
                      </div>
                    </div>
                  </div>
                </div>

                <div className="col-lg-9">
                  <div className="products-header">
                    <div className="products-info">
                      <h4>Products ({filteredProducts.length})</h4>
                    </div>
                    <div className="sort-section">
                      <label className="sort-label">Sort by:</label>
                      <select 
                        className="sort-select"
                        value={sortBy}
                        onChange={(e) => setSortBy(e.target.value)}
                      >
                        <option value="featured">Featured</option>
                        <option value="price-low">Price: Low to High</option>
                        <option value="price-high">Price: High to Low</option>
                        <option value="name">Name A-Z</option>
                        <option value="newest">Newest</option>
                      </select>
                    </div>
                  </div>

                  {filteredProducts.length === 0 ? (
                    <div className="empty-products">
                      <div className="empty-icon">No Results</div>
                      <h5>No products found</h5>
                      <p>Try adjusting your filters or search terms</p>
                    </div>
                  ) : (
                    <div className="products-grid">
                      {filteredProducts.map(product => (
                        <div key={product.productId} className="product-card">
                          <div className="product-image-container">
                            <img
                              src={product.imagePaths?.[0] ? `http://localhost:5108${product.imagePaths[0]}` : 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTgiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjNmM3NTdkIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4='}
                              alt={product.name}
                              className="product-image"
                              onError={(e) => {
                                (e.target as HTMLImageElement).src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTgiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjNmM3NTdkIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4=';
                              }}
                            />
                            {product.discountPrice && (
                              <div className="discount-badge">
                                {Math.round(((product.discountPrice - product.price) / product.discountPrice) * 100)}% OFF
                              </div>
                            )}
                          </div>
                          
                          <div className="product-content">
                            <h6 className="product-name">{product.name}</h6>
                            <p className="product-description">
                              {product.description?.substring(0, 80)}...
                            </p>
                            
                            <div className="product-meta">
                              <div className="price-section">
                                <span className="current-price">₹{product.price.toLocaleString()}</span>
                                {product.discountPrice && (
                                  <span className="original-price">₹{product.discountPrice.toLocaleString()}</span>
                                )}
                              </div>
                              <div className="stock-info">
                                <span className={`stock-badge ${product.stockQuantity < 10 ? 'low-stock' : 'in-stock'}`}>
                                  {product.stockQuantity < 10 ? `Only ${product.stockQuantity} left` : 'In Stock'}
                                </span>
                              </div>
                            </div>
                            
                            <div className="product-actions">
                              <button 
                                className="btn-add-cart"
                                onClick={() => handleAddToCart(product.productId)}
                                disabled={addingToCart.has(product.productId) || product.stockQuantity === 0}
                              >
                                {addingToCart.has(product.productId) ? (
                                  <div className="btn-spinner"></div>
                                ) : (
                                  product.stockQuantity === 0 ? 'Out of Stock' : 'Add to Cart'
                                )}
                              </button>
                              <button 
                                className="btn-view-details"
                                onClick={() => navigate(`/product/${product.productId}`)}
                              >
                                View Details
                              </button>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
      
      <style>{`
        .dashboard-container {
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
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 1rem;
        }
        
        .title-icon {
          font-size: 3rem;
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
        
        .tab-icon {
          font-size: 1.125rem;
        }
        
        .products-section {
          padding: 0 1rem;
        }
        
        .filters-card {
          background: white;
          border-radius: var(--radius-lg);
          padding: 1.5rem;
          box-shadow: var(--shadow-md);
          height: fit-content;
          position: sticky;
          top: 2rem;
        }
        
        .filters-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 1.5rem;
          padding-bottom: 1rem;
          border-bottom: 2px solid var(--gray-100);
        }
        
        .filters-header h5 {
          margin: 0;
          color: var(--gray-800);
          font-weight: 600;
        }
        
        .clear-filters-btn {
          background: none;
          border: none;
          color: var(--primary-color);
          font-size: 0.875rem;
          font-weight: 500;
          cursor: pointer;
          transition: color 0.2s ease;
        }
        
        .clear-filters-btn:hover {
          color: var(--primary-dark);
        }
        
        .filter-group {
          margin-bottom: 1.5rem;
        }
        
        .filter-label {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          font-weight: 500;
          color: var(--gray-700);
          margin-bottom: 0.75rem;
          font-size: 0.875rem;
        }
        
        .label-icon {
          font-size: 1rem;
        }
        
        .filter-input {
          width: 100%;
          border: 2px solid var(--gray-200);
          border-radius: var(--radius-md);
          padding: 0.75rem;
          font-size: 0.875rem;
          transition: all 0.2s ease;
        }
        
        .filter-input:focus {
          border-color: var(--primary-color);
          box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.1);
          outline: none;
        }
        
        .category-list {
          display: flex;
          flex-direction: column;
          gap: 0.5rem;
        }
        
        .category-item {
          background: var(--gray-50);
          border: 2px solid transparent;
          border-radius: var(--radius-md);
          padding: 0.75rem;
          text-align: left;
          font-size: 0.875rem;
          font-weight: 500;
          color: var(--gray-700);
          cursor: pointer;
          transition: all 0.2s ease;
        }
        
        .category-item:hover {
          background: var(--primary-light);
          color: var(--primary-dark);
        }
        
        .category-item.active {
          background: var(--primary-color);
          color: white;
          border-color: var(--primary-dark);
        }
        
        .price-inputs {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 0.5rem;
        }
        
        .products-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 1.5rem;
          padding: 1.5rem;
          background: white;
          border-radius: var(--radius-lg);
          box-shadow: var(--shadow-sm);
        }
        
        .products-info h4 {
          margin: 0;
          color: var(--gray-800);
          font-weight: 600;
        }
        
        .sort-section {
          display: flex;
          align-items: center;
          gap: 0.75rem;
        }
        
        .sort-label {
          font-weight: 500;
          color: var(--gray-700);
          font-size: 0.875rem;
        }
        
        .sort-select {
          border: 2px solid var(--gray-200);
          border-radius: var(--radius-md);
          padding: 0.5rem 0.75rem;
          font-size: 0.875rem;
          background: white;
          cursor: pointer;
        }
        
        .empty-products {
          text-align: center;
          padding: 4rem 2rem;
          background: white;
          border-radius: var(--radius-lg);
          box-shadow: var(--shadow-sm);
        }
        
        .empty-icon {
          font-size: 4rem;
          margin-bottom: 1rem;
          opacity: 0.5;
        }
        
        .products-grid {
          display: grid;
          grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
          gap: 1.5rem;
        }
        
        .product-card {
          background: white;
          border-radius: var(--radius-lg);
          overflow: hidden;
          box-shadow: var(--shadow-md);
          transition: all 0.3s ease;
        }
        
        .product-card:hover {
          transform: translateY(-5px);
          box-shadow: var(--shadow-xl);
        }
        
        .product-image-container {
          position: relative;
          height: 200px;
          overflow: hidden;
        }
        
        .product-image {
          width: 100%;
          height: 100%;
          object-fit: contain;
          background-color: #f8f9fa;
          transition: transform 0.3s ease;
        }
        
        .product-card:hover .product-image {
          transform: scale(1.05);
        }
        
        .discount-badge {
          position: absolute;
          top: 0.75rem;
          right: 0.75rem;
          background: var(--danger-color);
          color: white;
          padding: 0.25rem 0.5rem;
          border-radius: var(--radius-md);
          font-size: 0.75rem;
          font-weight: 600;
        }
        
        .product-content {
          padding: 1.25rem;
        }
        
        .product-name {
          font-size: 1.125rem;
          font-weight: 600;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
          line-height: 1.4;
        }
        
        .product-description {
          color: var(--gray-600);
          font-size: 0.875rem;
          line-height: 1.5;
          margin-bottom: 1rem;
        }
        
        .product-meta {
          margin-bottom: 1rem;
        }
        
        .price-section {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          margin-bottom: 0.5rem;
        }
        
        .current-price {
          font-size: 1.25rem;
          font-weight: 700;
          color: var(--primary-color);
        }
        
        .original-price {
          font-size: 1rem;
          color: var(--gray-500);
          text-decoration: line-through;
        }
        
        .stock-info {
          display: flex;
          align-items: center;
        }
        
        .stock-badge {
          font-size: 0.75rem;
          font-weight: 500;
          padding: 0.25rem 0.5rem;
          border-radius: var(--radius-sm);
        }
        
        .stock-badge.in-stock {
          background: var(--success-color);
          color: white;
        }
        
        .stock-badge.low-stock {
          background: var(--warning-color);
          color: white;
        }
        
        .product-actions {
          display: grid;
          gap: 0.75rem;
        }
        
        .btn-add-cart {
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 0.5rem;
          padding: 0.75rem 1rem;
          background: linear-gradient(135deg, var(--warning-color) 0%, #d97706 100%);
          color: white;
          border: none;
          border-radius: var(--radius-md);
          font-weight: 500;
          font-size: 0.875rem;
          cursor: pointer;
          transition: all 0.2s ease;
        }
        
        .btn-add-cart:hover:not(:disabled) {
          background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
          transform: translateY(-1px);
        }
        
        .btn-add-cart:disabled {
          opacity: 0.6;
          cursor: not-allowed;
        }
        
        .btn-view-details {
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 0.5rem;
          padding: 0.75rem 1rem;
          background: transparent;
          color: var(--primary-color);
          border: 2px solid var(--primary-color);
          border-radius: var(--radius-md);
          font-weight: 500;
          font-size: 0.875rem;
          cursor: pointer;
          transition: all 0.2s ease;
        }
        
        .btn-view-details:hover {
          background: var(--primary-color);
          color: white;
          transform: translateY(-1px);
        }
        
        .btn-icon {
          font-size: 1rem;
        }
        
        .btn-spinner {
          width: 16px;
          height: 16px;
          border: 2px solid rgba(255, 255, 255, 0.3);
          border-top: 2px solid white;
          border-radius: 50%;
          animation: spin 1s linear infinite;
        }
        
        @media (max-width: 768px) {
          .dashboard-title {
            font-size: 2rem;
            flex-direction: column;
            gap: 0.5rem;
          }
          
          .products-header {
            flex-direction: column;
            gap: 1rem;
            align-items: stretch;
          }
          
          .sort-section {
            justify-content: space-between;
          }
          
          .products-grid {
            grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
            gap: 1rem;
          }
          
          .filters-card {
            position: static;
            margin-bottom: 1.5rem;
          }
        }
      `}</style>
    </>
  );
};

export default CustomerDashboard;