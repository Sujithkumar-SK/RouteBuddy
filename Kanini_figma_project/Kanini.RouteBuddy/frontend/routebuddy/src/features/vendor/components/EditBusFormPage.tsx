import { useParams, useNavigate } from 'react-router-dom';
import EditBusForm from './EditBusForm';
import Layout from '../../../components/layout/Layout';

const EditBusFormPage = () => {
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
      <EditBusForm busId={parseInt(busId)} onBack={handleBack} />
    </Layout>
  );
};

export default EditBusFormPage;