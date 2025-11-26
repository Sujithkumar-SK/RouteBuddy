import React, { createContext, useContext, useState, type ReactNode } from 'react';

type NotificationType = 'success' | 'error' | 'warning' | 'info';

interface NotificationContextType {
  showNotification: (message: string, type?: NotificationType) => void;
  showConfirmation: (message: string, onConfirm: () => void) => void;
}

const NotificationContext = createContext<NotificationContextType | undefined>(undefined);

interface NotificationProviderProps {
  children: ReactNode;
}

export const NotificationProvider: React.FC<NotificationProviderProps> = ({ children }) => {
  const [notification, setNotification] = useState<{
    open: boolean;
    message: string;
    type: NotificationType;
  }>({
    open: false,
    message: '',
    type: 'info'
  });

  const [confirmation, setConfirmation] = useState<{
    open: boolean;
    message: string;
    onConfirm: () => void;
  }>({
    open: false,
    message: '',
    onConfirm: () => {}
  });

  const showNotification = (message: string, type: NotificationType = 'info') => {
    setNotification({
      open: true,
      message,
      type
    });
    
    setTimeout(() => {
      setNotification(prev => ({ ...prev, open: false }));
    }, 4000);
  };

  const showConfirmation = (message: string, onConfirm: () => void) => {
    setConfirmation({
      open: true,
      message,
      onConfirm
    });
  };

  const handleNotificationClose = () => {
    setNotification(prev => ({ ...prev, open: false }));
  };

  const handleConfirmationClose = () => {
    setConfirmation(prev => ({ ...prev, open: false }));
  };

  const handleConfirm = () => {
    confirmation.onConfirm();
    handleConfirmationClose();
  };

  const getBootstrapVariant = (type: NotificationType) => {
    switch (type) {
      case 'error': return 'danger';
      case 'warning': return 'warning';
      case 'success': return 'success';
      default: return 'info';
    }
  };

  return (
    <NotificationContext.Provider value={{ showNotification, showConfirmation }}>
      {children}
      
      {notification.open && (
        <div 
          className={`alert alert-${getBootstrapVariant(notification.type)} alert-dismissible fade show position-fixed`}
          style={{
            top: '20px',
            right: '20px',
            zIndex: 9999,
            minWidth: '300px',
            maxWidth: '400px'
          }}
        >
          {notification.message}
          <button 
            type="button" 
            className="btn-close" 
            onClick={handleNotificationClose}
          ></button>
        </div>
      )}

      {confirmation.open && (
        <div className="modal d-block" style={{ backgroundColor: 'rgba(0, 0, 0, 0.5)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Confirm Action</h5>
              </div>
              <div className="modal-body">
                <p>{confirmation.message}</p>
              </div>
              <div className="modal-footer">
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={handleConfirmationClose}
                >
                  Cancel
                </button>
                <button 
                  type="button" 
                  className="btn btn-danger" 
                  onClick={handleConfirm}
                >
                  Confirm
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </NotificationContext.Provider>
  );
};

export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (context === undefined) {
    throw new Error('useNotification must be used within a NotificationProvider');
  }
  return context;
};