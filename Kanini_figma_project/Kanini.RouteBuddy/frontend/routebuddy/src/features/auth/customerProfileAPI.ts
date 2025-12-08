import api from '../../services/api';

export interface CustomerProfile {
  customerId: number;
  firstName: string;
  middleName?: string;
  lastName: string;
  dateOfBirth: string;
  gender: number;
  email: string;
  phone: string;
  isActive: boolean;
  createdOn: string;
  updatedOn: string;
  profilePictureBase64?: string;
}

export interface UpdateCustomerProfile {
  firstName: string;
  middleName?: string;
  lastName: string;
  dateOfBirth: string;
  gender: number;
  phone: string;
}

export const customerProfileAPI = {
  getMyProfile: async (): Promise<CustomerProfile> => {
    const response = await api.get('/customer/profile/my-profile');
    return response.data;
  },

  updateMyProfile: async (data: UpdateCustomerProfile): Promise<{ message: string }> => {
    const response = await api.put('/customer/profile/my-profile', data);
    return response.data;
  },

  updateProfilePicture: async (file: File): Promise<{ message: string }> => {
    const formData = new FormData();
    formData.append('ProfilePicture', file);
    const response = await api.put('/customer/profile/my-profile/picture', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};