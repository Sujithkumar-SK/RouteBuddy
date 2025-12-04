import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { busAPI } from './busAPI';
import type { BusState, BusSearchRequest, BusFilteredSearchRequest, BusFilters } from './types';

const initialState: BusState = {
  searchResults: [],
  filteredResults: [],
  loading: false,
  error: null,
  searchParams: null,
  filters: {
    busTypes: [],
    amenities: [],
    priceRange: [0, 5000],
    departureTime: ['00:00', '23:59'],
    sortBy: 'price',
  },
};

export const searchBuses = createAsyncThunk(
  'bus/searchBuses',
  async (data: BusSearchRequest) => {
    const response = await busAPI.searchBuses(data);
    return response;
  }
);

export const searchBusesFiltered = createAsyncThunk(
  'bus/searchBusesFiltered',
  async (data: BusFilteredSearchRequest) => {
    const response = await busAPI.searchBusesFiltered(data);
    return response;
  }
);

const busSlice = createSlice({
  name: 'bus',
  initialState,
  reducers: {
    clearSearchResults: (state) => {
      state.searchResults = [];
      state.filteredResults = [];
      state.searchParams = null;
      state.error = null;
    },
    updateFilters: (state, action: PayloadAction<Partial<BusFilters>>) => {
      state.filters = { ...state.filters, ...action.payload };
      state.filteredResults = applyFilters(state.searchResults, state.filters);
    },
    resetFilters: (state) => {
      state.filters = initialState.filters;
      state.filteredResults = state.searchResults;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(searchBuses.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(searchBuses.fulfilled, (state, action) => {
        state.loading = false;
        state.searchResults = action.payload;
        state.filteredResults = action.payload;
        state.searchParams = action.meta.arg;
      })
      .addCase(searchBuses.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to search buses';
      })
      .addCase(searchBusesFiltered.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(searchBusesFiltered.fulfilled, (state, action) => {
        state.loading = false;
        state.searchResults = action.payload;
        state.filteredResults = action.payload;
      })
      .addCase(searchBusesFiltered.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to search buses';
      });
  },
});

const calculateDuration = (departureTime: string, arrivalTime: string): number => {
  const [depHour, depMin] = departureTime.split(':').map(Number);
  const [arrHour, arrMin] = arrivalTime.split(':').map(Number);
  
  const depMinutes = depHour * 60 + depMin;
  let arrMinutes = arrHour * 60 + arrMin;
  
  // Handle next day arrival (if arrival time is less than departure time)
  if (arrMinutes < depMinutes) {
    arrMinutes += 24 * 60; // Add 24 hours
  }
  
  return arrMinutes - depMinutes;
};

const applyFilters = (buses: any[], filters: BusFilters) => {
  let filtered = [...buses];

  if (filters.busTypes.length > 0) {
    filtered = filtered.filter(bus => filters.busTypes.includes(bus.busType));
  }

  if (filters.amenities.length > 0) {
    filtered = filtered.filter(bus => 
      filters.amenities.some(amenity => bus.amenities & amenity)
    );
  }

  filtered = filtered.filter(bus => 
    bus.basePrice >= filters.priceRange[0] && bus.basePrice <= filters.priceRange[1]
  );

  const [startTime, endTime] = filters.departureTime;
  filtered = filtered.filter(bus => {
    const depTime = bus.departureTime;
    return depTime >= startTime && depTime <= endTime;
  });

  filtered.sort((a, b) => {
    switch (filters.sortBy) {
      case 'price': return a.basePrice - b.basePrice;
      case 'departure': return a.departureTime.localeCompare(b.departureTime);
      case 'duration': {
        const aDuration = calculateDuration(a.departureTime, a.arrivalTime);
        const bDuration = calculateDuration(b.departureTime, b.arrivalTime);
        return aDuration - bDuration;
      }
      default: return 0;
    }
  });

  return filtered;
};

export const { clearSearchResults, updateFilters, resetFilters } = busSlice.actions;
export default busSlice.reducer;
