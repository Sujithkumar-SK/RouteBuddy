export const SeatType = {
  Seater: 1,
  SleeperLower: 2,
  SleeperUpper: 3,
  SemiSleeper: 4,
  SingleSeater: 5,
  SingleSemiSeater: 6,
} as const;

export const SeatPosition = {
  Window: 1,
  Aisle: 2,
  Middle: 3,
} as const;

export const PriceTier = {
  Base: 1,
  Premium: 2,
  Luxury: 3,
} as const;

export type SeatType = typeof SeatType[keyof typeof SeatType];
export type SeatPosition = typeof SeatPosition[keyof typeof SeatPosition];
export type PriceTier = typeof PriceTier[keyof typeof PriceTier];

export type SeatDto = {
  seatNumber: string;
  seatType: SeatType;
  seatPosition: SeatPosition;
  rowNumber: number;
  columnNumber: number;
  priceTier: PriceTier;
  isAvailable: boolean;
  isBooked: boolean;
  price: number;
};

export type BusInfoDto = {
  busName: string;
  busType: number;
  busAmenities: number;
  totalSeats: number;
  availableSeats: number;
  bookedSeats: number;
  basePrice: number;
};

export type SeatLayoutResponse = {
  scheduleId: number;
  travelDate: string;
  bus: BusInfoDto;
  seats: SeatDto[];
};

export type RouteStop = {
  routeStopId: number;
  stopId: number;
  stopName: string;
  landmark?: string;
  orderNumber: number;
  arrivalTime?: string;
  departureTime?: string;
};

export type PassengerDetails = {
  name: string;
  age: number;
  gender: number;
};

export type BookingRequest = {
  scheduleId: number;
  travelDate: string;
  seatNumbers: string[];
  passengers: PassengerDetails[];
  customerId: number;
  boardingStopId: number;
  droppingStopId: number;
};

export type BookingResponse = {
  bookingId: number;
  pnr: string;
  totalAmount: number;
  expiresAt: string;
  status: string;
};

export type SeatSelectionState = {
  seatLayout: SeatLayoutResponse | null;
  routeStops: RouteStop[];
  selectedSeats: string[];
  boardingStopId: number | null;
  droppingStopId: number | null;
  passengers: PassengerDetails[];
  totalPrice: number;
  loading: boolean;
  error: string | null;
  bookingTimer: number;
};