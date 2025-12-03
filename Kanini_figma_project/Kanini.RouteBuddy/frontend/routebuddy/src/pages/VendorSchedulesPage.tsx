import { useState } from 'react';
import { Container } from '@mui/material';
import Layout from '../components/layout/Layout';
import ScheduleList from '../features/vendor/components/ScheduleList';
import CreateScheduleForm from '../features/vendor/components/CreateScheduleForm';

const VendorSchedulesPage = () => {
  const [view, setView] = useState<'list' | 'create'>('list');

  const handleCreateNew = () => {
    setView('create');
  };

  const handleBack = () => {
    setView('list');
  };

  const handleSuccess = () => {
    setView('list');
  };

  return (
    <Layout>
      <Container maxWidth="lg" sx={{ py: 4 }}>
        {view === 'list' ? (
          <ScheduleList onCreateNew={handleCreateNew} />
        ) : (
          <CreateScheduleForm onBack={handleBack} onSuccess={handleSuccess} />
        )}
      </Container>
    </Layout>
  );
};

export default VendorSchedulesPage;