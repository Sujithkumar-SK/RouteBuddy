import { useState } from 'react';
import { Container } from '@mui/material';
import Layout from '../components/layout/Layout';
import ScheduleList from '../features/vendor/components/ScheduleList';
import CreateScheduleForm from '../features/vendor/components/CreateScheduleForm';
import CreateBulkScheduleForm from '../features/vendor/components/CreateBulkScheduleForm';

const VendorSchedulesPage = () => {
  const [view, setView] = useState<'list' | 'create' | 'bulk'>('list');

  const handleCreateNew = () => {
    setView('create');
  };

  const handleCreateBulk = () => {
    setView('bulk');
  };

  const handleBack = () => {
    setView('list');
  };

  const handleSuccess = () => {
    setView('list');
  };

  return (
    <Layout>
      <div className="schedule-container">
        <Container maxWidth="lg" sx={{ py: 4 }}>
          {view === 'list' ? (
            <ScheduleList onCreateNew={handleCreateNew} onCreateBulk={handleCreateBulk} />
          ) : view === 'create' ? (
            <CreateScheduleForm onBack={handleBack} onSuccess={handleSuccess} />
          ) : (
            <CreateBulkScheduleForm onBack={handleBack} onSuccess={handleSuccess} />
          )}
        </Container>
      </div>
    </Layout>
  );
};

export default VendorSchedulesPage;