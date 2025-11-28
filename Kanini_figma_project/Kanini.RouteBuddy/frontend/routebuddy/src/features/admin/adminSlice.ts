import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { adminAPI } from './adminAPI';
import type { AdminVendor, VendorRejection } from './adminAPI';

interface AdminState {
  vendors: AdminVendor[];
  pendingVendors: AdminVendor[];
  selectedVendor: AdminVendor | null;
  loading: {
    vendors: boolean;
    pending: boolean;
    action: boolean;
  };
  error: string | null;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

const initialState: AdminState = {
  vendors: [],
  pendingVendors: [],
  selectedVendor: null,
  loading: {
    vendors: false,
    pending: false,
    action: false,
  },
  error: null,
  totalCount: 0,
  pageNumber: 1,
  pageSize: 10,
};

export const fetchPendingVendors = createAsyncThunk(
  'admin/fetchPendingVendors',
  async ({ pageNumber = 1, pageSize = 10 }: { pageNumber?: number; pageSize?: number }, { rejectWithValue }) => {
    try {
      return await adminAPI.getPendingVendors(pageNumber, pageSize);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch pending vendors');
    }
  }
);

export const fetchAllVendors = createAsyncThunk(
  'admin/fetchAllVendors',
  async ({ pageNumber = 1, pageSize = 10 }: { pageNumber?: number; pageSize?: number }, { rejectWithValue }) => {
    try {
      return await adminAPI.getAllVendors(pageNumber, pageSize);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to fetch vendors');
    }
  }
);

export const filterVendors = createAsyncThunk(
  'admin/filterVendors',
  async (filters: { searchName?: string; isActive?: boolean; status?: number }, { rejectWithValue }) => {
    try {
      return await adminAPI.filterVendors(filters.searchName, filters.isActive, filters.status);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to filter vendors');
    }
  }
);

export const approveVendor = createAsyncThunk(
  'admin/approveVendor',
  async (vendorId: number, { rejectWithValue }) => {
    try {
      await adminAPI.approveVendor(vendorId);
      return vendorId;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to approve vendor');
    }
  }
);

export const rejectVendor = createAsyncThunk(
  'admin/rejectVendor',
  async ({ vendorId, rejectionData }: { vendorId: number; rejectionData: VendorRejection }, { rejectWithValue }) => {
    try {
      await adminAPI.rejectVendor(vendorId, rejectionData);
      return vendorId;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error || 'Failed to reject vendor');
    }
  }
);

const adminSlice = createSlice({
  name: 'admin',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setSelectedVendor: (state, action: PayloadAction<AdminVendor | null>) => {
      state.selectedVendor = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      // Pending Vendors
      .addCase(fetchPendingVendors.pending, (state) => {
        state.loading.pending = true;
        state.error = null;
      })
      .addCase(fetchPendingVendors.fulfilled, (state, action) => {
        state.loading.pending = false;
        state.pendingVendors = action.payload.data;
        state.totalCount = action.payload.totalCount;
      })
      .addCase(fetchPendingVendors.rejected, (state, action) => {
        state.loading.pending = false;
        state.error = action.payload as string;
      })

      // All Vendors
      .addCase(fetchAllVendors.pending, (state) => {
        state.loading.vendors = true;
        state.error = null;
      })
      .addCase(fetchAllVendors.fulfilled, (state, action) => {
        state.loading.vendors = false;
        state.vendors = action.payload.data;
        state.totalCount = action.payload.totalCount;
      })
      .addCase(fetchAllVendors.rejected, (state, action) => {
        state.loading.vendors = false;
        state.error = action.payload as string;
      })

      // Filter Vendors
      .addCase(filterVendors.fulfilled, (state, action) => {
        state.vendors = action.payload;
      })

      // Approve Vendor
      .addCase(approveVendor.pending, (state) => {
        state.loading.action = true;
        state.error = null;
      })
      .addCase(approveVendor.fulfilled, (state, action) => {
        state.loading.action = false;
        // Remove from pending vendors
        state.pendingVendors = state.pendingVendors.filter(v => v.vendorId !== action.payload);
        // Update in all vendors if present
        const vendorIndex = state.vendors.findIndex(v => v.vendorId === action.payload);
        if (vendorIndex !== -1) {
          state.vendors[vendorIndex].status = 1; // Active
          state.vendors[vendorIndex].isActive = true;
        }
      })
      .addCase(approveVendor.rejected, (state, action) => {
        state.loading.action = false;
        state.error = action.payload as string;
      })

      // Reject Vendor
      .addCase(rejectVendor.pending, (state) => {
        state.loading.action = true;
        state.error = null;
      })
      .addCase(rejectVendor.fulfilled, (state, action) => {
        state.loading.action = false;
        // Remove from pending vendors
        state.pendingVendors = state.pendingVendors.filter(v => v.vendorId !== action.payload);
        // Update in all vendors if present
        const vendorIndex = state.vendors.findIndex(v => v.vendorId === action.payload);
        if (vendorIndex !== -1) {
          state.vendors[vendorIndex].status = 4; // Rejected
          state.vendors[vendorIndex].isActive = false;
        }
      })
      .addCase(rejectVendor.rejected, (state, action) => {
        state.loading.action = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, setSelectedVendor } = adminSlice.actions;
export default adminSlice.reducer;