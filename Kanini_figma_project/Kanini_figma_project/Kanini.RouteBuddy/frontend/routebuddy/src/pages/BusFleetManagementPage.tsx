import React, { useState } from 'react';
import {
  Box,
  Container,
  Tabs,
  Tab,
  Typography,
} from '@mui/material';
import { useParams } from 'react-router-dom';
import Layout from '../components/layout/Layout';
import BusFleetList from '../features/vendor/components/BusFleetList';
import AddBusForm from '../features/vendor/components/AddBusForm';
import BusPhotoManager from '../features/vendor/components/BusPhotoManager';
import SeatLayoutConfig from '../features/vendor/components/SeatLayoutConfig';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`bus-fleet-tabpanel-${index}`}
      aria-labelledby={`bus-fleet-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
}

const BusFleetManagementPage: React.FC = () => {
  const { tab, busId } = useParams<{ tab?: string; busId?: string }>();
  const [activeTab, setActiveTab] = useState(() => {
    switch (tab) {
      case 'add': return 1;
      case 'photos': return 2;
      case 'layout': return 3;
      default: return 0;
    }
  });

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  return (
    <Layout>
      <Container maxWidth="xl">
        <Box sx={{ width: '100%' }}>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs value={activeTab} onChange={handleTabChange} aria-label="bus fleet management tabs">
              <Tab label="My Fleet" />
              <Tab label="Add Bus" />
              {busId && <Tab label="Photos" />}
              {busId && <Tab label="Seat Layout" />}
            </Tabs>
          </Box>

          <TabPanel value={activeTab} index={0}>
            <BusFleetList />
          </TabPanel>

          <TabPanel value={activeTab} index={1}>
            <AddBusForm />
          </TabPanel>

          {busId && (
            <>
              <TabPanel value={activeTab} index={2}>
                <BusPhotoManager />
              </TabPanel>

              <TabPanel value={activeTab} index={3}>
                <SeatLayoutConfig />
              </TabPanel>
            </>
          )}
        </Box>
      </Container>
    </Layout>
  );
};

export default BusFleetManagementPage;