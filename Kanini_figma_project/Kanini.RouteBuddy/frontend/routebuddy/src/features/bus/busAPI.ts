import api from '../../services/api';
import type { BusSearchRequest, BusFilteredSearchRequest, BusSearchResponse } from './types';

export const busAPI = {
  searchBuses: async (data: BusSearchRequest): Promise<BusSearchResponse[]> => {
    const response = await api.post('/Bus_Search_Book_/search', data);
    return response.data;
  },

  searchBusesFiltered: async (data: BusFilteredSearchRequest): Promise<BusSearchResponse[]> => {
    const response = await api.post('/Bus_Search_Book_/search/filtered', data);
    return response.data;
  },
};
