import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { customerProfileAPI, type CustomerProfile, type UpdateCustomerProfile } from './customerProfileAPI';

interface CustomerProfileState {
  profile: CustomerProfile | null;
  loading: boolean;
  error: string | null;
  updateSuccess: boolean;
}

const initialState: CustomerProfileState = {
  profile: null,
  loading: false,
  error: null,
  updateSuccess: false,
};

export const fetchCustomerProfile = createAsyncThunk(
  'customerProfile/fetchProfile',
  async (_, { rejectWithValue }) => {
    try {
      console.log('fetchCustomerProfile: Making API call...');
      const response = await customerProfileAPI.getMyProfile();
      console.log('fetchCustomerProfile: API response:', response);
      return response;
    } catch (error: any) {
      console.error('fetchCustomerProfile: API error:', error);
      const errorMsg = error.response?.data?.error || error.message || 'Failed to fetch profile';
      return rejectWithValue(errorMsg);
    }
  }
);

export const updateCustomerProfile = createAsyncThunk(
  'customerProfile/updateProfile',
  async (data: UpdateCustomerProfile, { rejectWithValue }) => {
    try {
      const response = await customerProfileAPI.updateMyProfile(data);
      return { ...data, message: response.message };
    } catch (error: any) {
      const errorMsg = error.response?.data?.error || error.message || 'Failed to update profile';
      return rejectWithValue(errorMsg);
    }
  }
);

export const updateProfilePicture = createAsyncThunk(
  'customerProfile/updateProfilePicture',
  async (file: File, { rejectWithValue }) => {
    try {
      const response = await customerProfileAPI.updateProfilePicture(file);
      return response.message;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error || error.message || 'Failed to update profile picture';
      return rejectWithValue(errorMsg);
    }
  }
);

const customerProfileSlice = createSlice({
  name: 'customerProfile',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    clearUpdateSuccess: (state) => {
      state.updateSuccess = false;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCustomerProfile.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCustomerProfile.fulfilled, (state, action) => {
        state.loading = false;
        state.profile = action.payload;
      })
      .addCase(fetchCustomerProfile.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(updateCustomerProfile.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.updateSuccess = false;
      })
      .addCase(updateCustomerProfile.fulfilled, (state, action) => {
        state.loading = false;
        state.updateSuccess = true;
        if (state.profile) {
          state.profile = {
            ...state.profile,
            firstName: action.payload.firstName,
            middleName: action.payload.middleName,
            lastName: action.payload.lastName,
            dateOfBirth: action.payload.dateOfBirth,
            gender: action.payload.gender,
            phone: action.payload.phone,
            updatedOn: new Date().toISOString(),
          };
        }
      })
      .addCase(updateCustomerProfile.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
        state.updateSuccess = false;
      })
      .addCase(updateProfilePicture.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateProfilePicture.fulfilled, (state) => {
        state.loading = false;
        // Refresh profile to get updated picture
      })
      .addCase(updateProfilePicture.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, clearUpdateSuccess } = customerProfileSlice.actions;
export default customerProfileSlice.reducer;