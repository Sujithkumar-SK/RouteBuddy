import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { cartService } from '../Services/cartService';
import { useNotification } from '../context/notificationContext';
import type { CartSummary, CartItem } from '../types/cart';

const Cart: React.FC = () => {
  const navigate = useNavigate();
  const { showNotification, showConfirmation } = useNotification();
  const [cart, setCart] = useState<CartSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [updatingItems, setUpdatingItems] = useState<Set<number>>(new Set());

  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = async () => {
    try {
      setLoading(true);
      const cartData = await cartService.getCart();
      setCart(cartData);
    } catch (err: any) {
      setError('Failed to load cart');
      console.error('Error loading cart:', err);
    } finally {
      setLoading(false);
    }
  };

  const updateQuantity = async (cartId: number, newQuantity: number) => {
    if (newQuantity < 1) return;

    try {
      setUpdatingItems(prev => new Set(prev).add(cartId));
      await cartService.updateCartItem(cartId, { quantity: newQuantity });
      await loadCart();
      window.dispatchEvent(new CustomEvent('cartUpdated'));
    } catch (error: any) {
      console.error('Error updating quantity:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to update quantity';
      showNotification(errorMsg, 'error');
    } finally {
      setUpdatingItems(prev => {
        const newSet = new Set(prev);
        newSet.delete(cartId);
        return newSet;
      });
    }
  };

  const removeItem = async (cartId: number) => {
    try {
      setUpdatingItems(prev => new Set(prev).add(cartId));
      await cartService.removeCartItem(cartId);
      await loadCart();
      window.dispatchEvent(new CustomEvent('cartUpdated'));
      showNotification('Item removed from cart', 'success');
    } catch (error: any) {
      console.error('Error removing item:', error);
      const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to remove item';
      showNotification(errorMsg, 'error');
    } finally {
      setUpdatingItems(prev => {
        const newSet = new Set(prev);
        newSet.delete(cartId);
        return newSet;
      });
    }
  };

  const clearCart = async () => {
    showConfirmation('Are you sure you want to clear your cart?', async () => {
      try {
        setLoading(true);
        await cartService.clearCart();
        await loadCart();
        window.dispatchEvent(new CustomEvent('cartUpdated'));
        showNotification('Cart cleared successfully', 'success');
      } catch (error: any) {
        console.error('Error clearing cart:', error);
        const errorMsg = error.response?.data?.description || error.response?.data?.error?.description || 'Failed to clear cart';
        showNotification(errorMsg, 'error');
      } finally {
        setLoading(false);
      }
    });
  };



  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-spinner">
          <div className="spinner"></div>
          <p>Loading your cart...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="error-container">
        <div className="error-content">
          <div className="error-icon">⚠️</div>
          <h3>Oops! Something went wrong</h3>
          <p>{error}</p>
          <button className="btn-primary" onClick={() => navigate('/dashboard')}>
            Continue Shopping
          </button>
        </div>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="empty-cart-container">
        <div className="empty-cart-content">
          <div className="empty-cart-animation">
            <div className="cart-icon">Cart</div>
          </div>
          <h2>Your Cart is Empty</h2>
          <p>Looks like you haven't added any items to your cart yet.</p>
          <button
            className="btn-start-shopping"
            onClick={() => navigate('/dashboard')}
          >
            Start Shopping
          </button>
        </div>
      </div>
    );
  }

  return (
    <>
      <div className="cart-container">
        <div className="cart-header">
          <div className="container-fluid">
            <div className="header-content">
              <div className="cart-title-section">
                <h1 className="cart-title">
                  Shopping Cart
                </h1>
                <p className="cart-subtitle">{cart.totalItems} items in your cart</p>
              </div>
              <button
                className="btn-continue-shopping"
                onClick={() => navigate('/dashboard')}
              >
                Continue Shopping
              </button>
            </div>
          </div>
        </div>

        <div className="container-fluid">
          <div className="row g-4">
            <div className="col-lg-8">
              <div className="cart-items-section">
                <div className="section-header">
                  <h5>Items in Cart</h5>
                  <button
                    className="btn-clear-cart"
                    onClick={clearCart}
                    disabled={loading}
                  >
                    Clear Cart
                  </button>
                </div>

                <div className="cart-items">
                  {cart.items.map((item: CartItem) => (
                    <div key={item.cartId} className="cart-item">
                      <div className="item-image-section">
                        <img
                          src={item.productImage ? `http://localhost:5108${item.productImage}` : 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjhmOWZhIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTIiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIiBmaWxsPSIjNmM3NTdkIj5JbWFnZTwvdGV4dD48L3N2Zz4='}
                          alt={item.productName}
                          className="item-image"
                        />
                      </div>

                      <div className="item-details">
                        <h6 className="item-name">{item.productName}</h6>
                        <div className="item-meta">
                          <span className="item-sku">SKU: {item.productSKU}</span>
                          <span className="item-vendor">by {item.vendorName}</span>
                        </div>
                        {item.stockQuantity < 10 && (
                          <div className="stock-warning">
                            <span className="warning-icon">⚠️</span>
                            Only {item.stockQuantity} left in stock
                          </div>
                        )}
                      </div>

                      <div className="item-price">
                        <div className="price-display">
                          <span className="current-price">₹{item.price.toLocaleString()}</span>
                          {item.discountPrice && (
                            <span className="original-price">₹{item.discountPrice.toLocaleString()}</span>
                          )}
                        </div>
                      </div>

                      <div className="item-quantity">
                        <div className="quantity-controls">
                          <button
                            className="qty-btn qty-decrease"
                            onClick={() => updateQuantity(item.cartId, item.quantity - 1)}
                            disabled={item.quantity <= 1 || updatingItems.has(item.cartId)}
                          >
                            −
                          </button>
                          <div className="quantity-display">
                            {updatingItems.has(item.cartId) ? (
                              <div className="qty-spinner"></div>
                            ) : (
                              item.quantity
                            )}
                          </div>
                          <button
                            className="qty-btn qty-increase"
                            onClick={() => updateQuantity(item.cartId, item.quantity + 1)}
                            disabled={item.quantity >= item.stockQuantity || updatingItems.has(item.cartId)}
                          >
                            +
                          </button>
                        </div>
                      </div>

                      <div className="item-total">
                        <div className="total-price">₹{item.totalPrice.toLocaleString()}</div>
                        <button
                          className="btn-remove-item"
                          onClick={() => removeItem(item.cartId)}
                          disabled={updatingItems.has(item.cartId)}
                        >
                          Remove
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>

            <div className="col-lg-4">
              <div className="cart-summary">
                <div className="summary-header">
                  <h5>Order Summary</h5>
                </div>

                <div className="summary-content">
                  <div className="summary-row">
                    <span>Subtotal ({cart.totalItems} items)</span>
                    <span>₹{cart.subTotal.toLocaleString()}</span>
                  </div>

                  {cart.totalDiscount > 0 && (
                    <div className="summary-row discount-row">
                      <span>
                        Discount
                      </span>
                      <span className="discount-amount">-₹{cart.totalDiscount.toLocaleString()}</span>
                    </div>
                  )}

                  <div className="summary-row shipping-row">
                    <span>
                      Shipping
                    </span>
                    <span className="free-shipping">FREE</span>
                  </div>

                  <div className="summary-divider"></div>

                  <div className="summary-row total-row">
                    <span>Total</span>
                    <span className="total-amount">₹{cart.grandTotal.toLocaleString()}</span>
                  </div>

                  <button
                    className="btn-checkout"
                    onClick={() => navigate('/checkout')}
                  >
                    Proceed to Checkout
                  </button>

                  <div className="security-info">
                    <span>Secure checkout guaranteed</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <style>{`
        .cart-container {
          min-height: 100vh;
          background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
        }
        
        .loading-container, .error-container, .empty-cart-container {
          min-height: 100vh;
          display: flex;
          align-items: center;
          justify-content: center;
          background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
        }
        
        .loading-spinner, .error-content, .empty-cart-content {
          text-align: center;
          padding: 2rem;
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
        
        .error-icon {
          font-size: 4rem;
          margin-bottom: 1rem;
        }
        
        .empty-cart-animation {
          position: relative;
          margin-bottom: 2rem;
        }
        
        .cart-icon {
          font-size: 5rem;
          margin-bottom: 1rem;
          display: block;
        }
        
        .empty-indicator {
          position: absolute;
          top: 0;
          right: 50%;
          font-size: 2rem;
          animation: float 2s ease-in-out infinite;
        }
        
        @keyframes float {
          0%, 100% { transform: translateY(0px); }
          50% { transform: translateY(-10px); }
        }
        
        .btn-start-shopping, .btn-primary {
          display: inline-flex;
          align-items: center;
          gap: 0.5rem;
          padding: 1rem 2rem;
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
          color: white;
          border: none;
          border-radius: var(--radius-lg);
          font-weight: 600;
          font-size: 1rem;
          cursor: pointer;
          transition: all 0.3s ease;
          text-decoration: none;
        }
        
        .btn-start-shopping:hover, .btn-primary:hover {
          background: linear-gradient(135deg, var(--primary-dark) 0%, #3730a3 100%);
          transform: translateY(-2px);
          box-shadow: var(--shadow-lg);
        }
        
        .cart-header {
          background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-dark) 100%);
          color: white;
          padding: 2rem 0;
          margin-bottom: 2rem;
        }
        
        .header-content {
          display: flex;
          justify-content: space-between;
          align-items: center;
          flex-wrap: wrap;
          gap: 1rem;
        }
        
        .cart-title {
          font-size: 2rem;
          font-weight: 700;
          margin: 0;
          display: flex;
          align-items: center;
          gap: 0.75rem;
        }
        
        .title-icon {
          font-size: 2.5rem;
        }
        
        .cart-subtitle {
          margin: 0;
          opacity: 0.9;
          font-size: 1rem;
        }
        
        .btn-continue-shopping {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          padding: 0.75rem 1.5rem;
          background: rgba(255, 255, 255, 0.2);
          color: white;
          border: 2px solid rgba(255, 255, 255, 0.3);
          border-radius: var(--radius-md);
          font-weight: 500;
          cursor: pointer;
          transition: all 0.2s ease;
          backdrop-filter: blur(10px);
        }
        
        .btn-continue-shopping:hover {
          background: rgba(255, 255, 255, 0.3);
          border-color: rgba(255, 255, 255, 0.5);
          transform: translateY(-1px);
        }
        
        .cart-items-section {
          background: white;
          border-radius: var(--radius-lg);
          padding: 1.5rem;
          box-shadow: var(--shadow-md);
        }
        
        .section-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 1.5rem;
          padding-bottom: 1rem;
          border-bottom: 2px solid var(--gray-100);
        }
        
        .section-header h5 {
          margin: 0;
          color: var(--gray-800);
          font-weight: 600;
        }
        
        .btn-clear-cart {
          display: flex;
          align-items: center;
          gap: 0.5rem;
          padding: 0.5rem 1rem;
          background: var(--danger-color);
          color: white;
          border: none;
          border-radius: var(--radius-md);
          font-size: 0.875rem;
          font-weight: 500;
          cursor: pointer;
          transition: all 0.2s ease;
        }
        
        .btn-clear-cart:hover:not(:disabled) {
          background: #dc2626;
          transform: translateY(-1px);
        }
        
        .cart-items {
          display: flex;
          flex-direction: column;
          gap: 1rem;
        }
        
        .cart-item {
          display: grid;
          grid-template-columns: 100px 1fr auto auto auto;
          gap: 1rem;
          align-items: center;
          padding: 1rem;
          background: var(--gray-50);
          border-radius: var(--radius-md);
          transition: all 0.2s ease;
        }
        
        .cart-item:hover {
          background: var(--gray-100);
          transform: translateY(-1px);
        }
        
        .item-image {
          width: 80px;
          height: 80px;
          object-fit: cover;
          border-radius: var(--radius-md);
        }
        
        .item-details {
          min-width: 0;
        }
        
        .item-name {
          font-size: 1rem;
          font-weight: 600;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
          line-height: 1.4;
        }
        
        .item-meta {
          display: flex;
          flex-direction: column;
          gap: 0.25rem;
          font-size: 0.875rem;
          color: var(--gray-600);
        }
        
        .stock-warning {
          display: flex;
          align-items: center;
          gap: 0.25rem;
          margin-top: 0.5rem;
          font-size: 0.75rem;
          color: var(--warning-color);
          font-weight: 500;
        }
        
        .price-display {
          text-align: right;
        }
        
        .current-price {
          display: block;
          font-size: 1.125rem;
          font-weight: 600;
          color: var(--gray-800);
        }
        
        .original-price {
          display: block;
          font-size: 0.875rem;
          color: var(--gray-500);
          text-decoration: line-through;
          margin-top: 0.25rem;
        }
        
        .quantity-controls {
          display: flex;
          align-items: center;
          background: white;
          border-radius: var(--radius-md);
          border: 2px solid var(--gray-200);
          overflow: hidden;
        }
        
        .qty-btn {
          width: 36px;
          height: 36px;
          border: none;
          background: var(--gray-100);
          color: var(--gray-700);
          font-weight: 600;
          cursor: pointer;
          transition: all 0.2s ease;
          display: flex;
          align-items: center;
          justify-content: center;
        }
        
        .qty-btn:hover:not(:disabled) {
          background: var(--primary-color);
          color: white;
        }
        
        .qty-btn:disabled {
          opacity: 0.5;
          cursor: not-allowed;
        }
        
        .quantity-display {
          width: 50px;
          height: 36px;
          display: flex;
          align-items: center;
          justify-content: center;
          font-weight: 600;
          color: var(--gray-800);
          background: white;
        }
        
        .qty-spinner {
          width: 16px;
          height: 16px;
          border: 2px solid var(--gray-200);
          border-top: 2px solid var(--primary-color);
          border-radius: 50%;
          animation: spin 1s linear infinite;
        }
        
        .item-total {
          text-align: right;
        }
        
        .total-price {
          font-size: 1.25rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 0.5rem;
        }
        
        .btn-remove-item {
          display: flex;
          align-items: center;
          gap: 0.25rem;
          padding: 0.375rem 0.75rem;
          background: transparent;
          color: var(--danger-color);
          border: 1px solid var(--danger-color);
          border-radius: var(--radius-sm);
          font-size: 0.75rem;
          font-weight: 500;
          cursor: pointer;
          transition: all 0.2s ease;
        }
        
        .btn-remove-item:hover:not(:disabled) {
          background: var(--danger-color);
          color: white;
        }
        
        .cart-summary {
          background: white;
          border-radius: var(--radius-lg);
          box-shadow: var(--shadow-md);
          position: sticky;
          top: 2rem;
        }
        
        .summary-header {
          padding: 1.5rem 1.5rem 0;
        }
        
        .summary-header h5 {
          margin: 0;
          color: var(--gray-800);
          font-weight: 600;
        }
        
        .summary-content {
          padding: 1.5rem;
        }
        
        .summary-row {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 1rem;
          font-size: 0.875rem;
        }
        
        .discount-row {
          color: var(--success-color);
        }
        
        .discount-amount {
          font-weight: 600;
        }
        
        .shipping-row {
          color: var(--gray-600);
        }
        
        .free-shipping {
          color: var(--success-color);
          font-weight: 600;
        }
        
        .summary-divider {
          height: 1px;
          background: var(--gray-200);
          margin: 1.5rem 0;
        }
        
        .total-row {
          font-size: 1.125rem;
          font-weight: 700;
          color: var(--gray-800);
          margin-bottom: 1.5rem;
        }
        
        .total-amount {
          color: var(--primary-color);
        }
        
        .btn-checkout {
          width: 100%;
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 0.5rem;
          padding: 1rem;
          background: linear-gradient(135deg, var(--success-color) 0%, #059669 100%);
          color: white;
          border: none;
          border-radius: var(--radius-md);
          font-size: 1rem;
          font-weight: 600;
          cursor: pointer;
          transition: all 0.3s ease;
          margin-bottom: 1rem;
        }
        
        .btn-checkout:hover {
          background: linear-gradient(135deg, #059669 0%, #047857 100%);
          transform: translateY(-2px);
          box-shadow: var(--shadow-lg);
        }
        
        .security-info {
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 0.5rem;
          font-size: 0.875rem;
          color: var(--gray-600);
        }
        
        .btn-icon {
          font-size: 1rem;
        }
        
        @media (max-width: 768px) {
          .cart-item {
            grid-template-columns: 1fr;
            text-align: center;
            gap: 1rem;
          }
          
          .item-details {
            order: 1;
          }
          
          .item-image-section {
            order: 0;
            justify-self: center;
          }
          
          .item-price {
            order: 2;
          }
          
          .item-quantity {
            order: 3;
            justify-self: center;
          }
          
          .item-total {
            order: 4;
            text-align: center;
          }
          
          .header-content {
            flex-direction: column;
            text-align: center;
          }
          
          .cart-title {
            font-size: 1.5rem;
          }
        }
      `}</style>
    </>
  );
};

export default Cart;