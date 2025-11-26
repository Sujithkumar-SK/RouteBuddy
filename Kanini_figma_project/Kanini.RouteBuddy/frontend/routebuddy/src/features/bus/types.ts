export type BusSearchRequest = {
  source: string;
  destination: string;
  travelDate: string;
};

export type BusFilteredSearchRequest = {
  source: string;
  destination: string;
  travelDate: string;
  departureTimeFrom?: string;
  departureTimeTo?: string;
  minPrice?: number;
  maxPrice?: number;
  busTypes?: number[];
  amenities?: number[];
  sortBy?: string;
};

export type BusSearchResponse = {
  scheduleId: number;
  busId: number;
  busName: string;
  busType: number;
  totalSeats: number;
  availableSeats: number;
  source: string;
  destination: string;
  travelDate: string;
  departureTime: string;
  arrivalTime: string;
  basePrice: number;
  amenities: number;
  vendorName: string;
};

export type BusFilters = {
  busTypes: number[];
  amenities: number[];
  priceRange: [number, number];
  departureTime: [string, string];
  sortBy: string;
};

export type BusState = {
  searchResults: BusSearchResponse[];
  filteredResults: BusSearchResponse[];
  loading: boolean;
  error: string | null;
  searchParams: BusSearchRequest | null;
  filters: BusFilters;
};
