import { useParams, useNavigate } from 'react-router-dom';
import BusPhotoManager from './BusPhotoManager';
import Layout from '../../../components/layout/Layout';

const BusPhotoManagerPage = () => {
  const { busId } = useParams<{ busId: string }>();
  const navigate = useNavigate();

  const handleBack = () => {
    navigate('/vendor/fleet');
  };

  if (!busId) {
    return <div>Bus ID not found</div>;
  }

  return (
    <Layout>
      <BusPhotoManager busId={parseInt(busId)} onBack={handleBack} />
    </Layout>
  );
};

export default BusPhotoManagerPage;