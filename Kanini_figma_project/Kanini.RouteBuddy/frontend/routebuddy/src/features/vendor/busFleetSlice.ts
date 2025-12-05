import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { busFleetAPI } from './busFleetAPI';
import type { BusFleetItem, CreateBusRequest, UpdateBusRequest, BusPhoto, SeatLayoutTemplate } from './busFleetAPI';

interface BusFleetState {
  buses: BusFleetItem[];
  currentBus: BusFleetItem | null;
  busPhotos: BusPhoto[];
  seatLayoutTemplates: SeatLayoutTemplate[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  loading: boolean;
  error: string | null;
  searchTerm: string;
  statusFilter: number | null;
}

const initialState: BusFleetState = {
  buses: [],
  currentBus: null,
  busPhotos: [],
  seatLayoutTemplates: [],
  totalCount: 0,
  currentPage: 1,
  pageSize: 10,
  loading: false,
  error: null,
  searchTerm: '',
  statusFilter: null,
};

export const fetchBusFleet = createAsyncThunk(
  'busFleet/fetchBusFleet',
  async (params: { pageNumber?: number; pageSize?: number; status?: number; search?: string }, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.getBusFleet(params);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch bus fleet');
    }
  }
);

export const createBus = createAsyncThunk(
  'busFleet/createBus',
  async (data: CreateBusRequest, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.createBus(data);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create bus');
    }
  }
);

export const updateBus = createAsyncThunk(
  'busFleet/updateBus',
  async ({ busId, data }: { busId: number; data: UpdateBusRequest }, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.updateBus(busId, data);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update bus');
    }
  }
);

export const deleteBus = createAsyncThunk(
  'busFleet/deleteBus',
  async (busId: number, { rejectWithValue }) => {
    try {
      await busFleetAPI.deleteBus(busId);
      return busId;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete bus');
    }
  }
);

export const fetchBusDetails = createAsyncThunk(
  'busFleet/fetchBusDetails',
  async (busId: number, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.getBusDetails(busId);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch bus details');
    }
  }
);

export const changeBusStatus = createAsyncThunk(
  'busFleet/changeBusStatus',
  async ({ busId, action }: { busId: number; action: 'activate' | 'deactivate' | 'maintenance' }, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.changeBusStatus(busId, action);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to change bus status');
    }
  }
);

export const fetchBusPhotos = createAsyncThunk(
  'busFleet/fetchBusPhotos',
  async (busId: number, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.getBusPhotos(busId);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch bus photos');
    }
  }
);

export const addBusPhoto = createAsyncThunk(
  'busFleet/addBusPhoto',
  async ({ busId, photo, caption }: { busId: number; photo: File; caption?: string }, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.addBusPhoto(busId, photo, caption);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to add bus photo');
    }
  }
);

export const deleteBusPhoto = createAsyncThunk(
  'busFleet/deleteBusPhoto',
  async (photoId: number, { rejectWithValue }) => {
    try {
      await busFleetAPI.deleteBusPhoto(photoId);
      return photoId;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete bus photo');
    }
  }
);

export const fetchSeatLayoutTemplates = createAsyncThunk(
  'busFleet/fetchSeatLayoutTemplates',
  async (_, { rejectWithValue }) => {
    try {
      const response = await busFleetAPI.getSeatLayoutTemplates();
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch seat layout templates');
    }
  }
);

const busFleetSlice = createSlice({
  name: 'busFleet',
  initialState,
  reducers: {
    setSearchTerm: (state, action) => {
      state.searchTerm = action.payload;
    },
    setStatusFilter: (state, action) => {
      state.statusFilter = action.payload;
    },
    setCurrentPage: (state, action) => {
      state.currentPage = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
    clearCurrentBus: (state) => {
      state.currentBus = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch Bus Fleet
      .addCase(fetchBusFleet.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchBusFleet.fulfilled, (state, action) => {
        state.loading = false;
        // Handle paginated response structure
        const responseData = action.payload.data || action.payload;
        state.buses = responseData.data || [];
        state.totalCount = responseData.totalCount || 0;
        state.currentPage = responseData.pageNumber || 1;
        state.pageSize = responseData.pageSize || 10;
      })
      .addCase(fetchBusFleet.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      
      // Create Bus
      .addCase(createBus.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createBus.fulfilled, (state, action) => {
        state.loading = false;
        // Ensure buses is an array before using unshift
        if (!Array.isArray(state.buses)) {
          state.buses = [];
        }
        // Handle single bus response structure
        const newBus = action.payload.data || action.payload;
        if (newBus) {
          state.buses.unshift(newBus);
          state.totalCount += 1;
        }
      })
      .addCase(createBus.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      
      // Update Bus
      .addCase(updateBus.fulfilled, (state, action) => {
        const index = state.buses.findIndex(bus => bus.busId === action.payload.data.busId);
        if (index !== -1) {
          state.buses[index] = action.payload.data;
        }
        if (state.currentBus?.busId === action.payload.data.busId) {
          state.currentBus = action.payload.data;
        }
      })
      
      // Delete Bus
      .addCase(deleteBus.fulfilled, (state, action) => {
        state.buses = state.buses.filter(bus => bus.busId !== action.payload);
        if (state.currentBus?.busId === action.payload) {
          state.currentBus = null;
        }
      })
      
      // Fetch Bus Details
      .addCase(fetchBusDetails.fulfilled, (state, action) => {
        state.currentBus = action.payload.data;
      })
      
      // Change Bus Status
      .addCase(changeBusStatus.fulfilled, (state, action) => {
        const index = state.buses.findIndex(bus => bus.busId === action.payload.data.busId);
        if (index !== -1) {
          state.buses[index] = action.payload.data;
        }
        if (state.currentBus?.busId === action.payload.data.busId) {
          state.currentBus = action.payload.data;
        }
      })
      
      // Fetch Bus Photos
      .addCase(fetchBusPhotos.fulfilled, (state, action) => {
        state.busPhotos = action.payload || [];
      })
      
      // Add Bus Photo
      .addCase(addBusPhoto.fulfilled, (state, action) => {
        if (action.payload.data) {
          state.busPhotos.push(action.payload.data);
        }
      })
      
      // Delete Bus Photo
      .addCase(deleteBusPhoto.fulfilled, (state, action) => {
        state.busPhotos = state.busPhotos.filter(photo => photo.busPhotoId !== action.payload);
      })
      
      // Fetch Seat Layout Templates
      .addCase(fetchSeatLayoutTemplates.fulfilled, (state, action) => {
        state.seatLayoutTemplates = action.payload || [];
      });
  },
});

export const { setSearchTerm, setStatusFilter, setCurrentPage, clearError, clearCurrentBus } = busFleetSlice.actions;
export default busFleetSlice.reducer;