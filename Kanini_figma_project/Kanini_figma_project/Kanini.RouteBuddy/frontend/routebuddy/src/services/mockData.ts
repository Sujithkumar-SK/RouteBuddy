import { BusSearchResponse } from '../features/bus/types';
import { BusType, BusAmenities } from '../utils/constants';

export const mockBusData: BusSearchResponse[] = [
  {
    scheduleId: 1,
    busId: 1,
    busName: "Shivneri Travels Express",
    busType: BusType.Volvo,
    totalSeats: 40,
    availableSeats: 35,
    source: "Mumbai",
    destination: "Pune",
    travelDate: "2024-01-15",
    departureTime: "06:00:00",
    arrivalTime: "09:30:00",
    basePrice: 850,
    amenities: BusAmenities.AC | BusAmenities.WiFi | BusAmenities.Charging,
    vendorName: "Shivneri Travels"
  },
  {
    scheduleId: 2,
    busId: 2,
    busName: "Red Bus Luxury",
    busType: BusType.Luxury,
    totalSeats: 32,
    availableSeats: 28,
    source: "Mumbai",
    destination: "Pune",
    travelDate: "2024-01-15",
    departureTime: "07:30:00",
    arrivalTime: "11:00:00",
    basePrice: 1200,
    amenities: BusAmenities.AC | BusAmenities.WiFi | BusAmenities.Charging | BusAmenities.Entertainment | BusAmenities.Snacks,
    vendorName: "Red Bus Services"
  },
  {
    scheduleId: 3,
    busId: 3,
    busName: "Orange Travels Sleeper",
    busType: BusType.Sleeper,
    totalSeats: 36,
    availableSeats: 20,
    source: "Mumbai",
    destination: "Pune",
    travelDate: "2024-01-15",
    departureTime: "22:00:00",
    arrivalTime: "05:30:00",
    basePrice: 650,
    amenities: BusAmenities.AC | BusAmenities.Blanket | BusAmenities.Pillow,
    vendorName: "Orange Travels"
  },
  {
    scheduleId: 4,
    busId: 4,
    busName: "VRL Travels AC",
    busType: BusType.AC,
    totalSeats: 45,
    availableSeats: 42,
    source: "Mumbai",
    destination: "Pune",
    travelDate: "2024-01-15",
    departureTime: "14:00:00",
    arrivalTime: "17:30:00",
    basePrice: 750,
    amenities: BusAmenities.AC | BusAmenities.Charging | BusAmenities.WashRoom,
    vendorName: "VRL Travels"
  },
  {
    scheduleId: 5,
    busId: 5,
    busName: "SRS Travels Non-AC",
    busType: BusType.NonAC,
    totalSeats: 50,
    availableSeats: 45,
    source: "Mumbai",
    destination: "Pune",
    travelDate: "2024-01-15",
    departureTime: "08:00:00",
    arrivalTime: "12:00:00",
    basePrice: 450,
    amenities: BusAmenities.Charging,
    vendorName: "SRS Travels"
  }
];

// Mock API delay
export const mockApiDelay = (ms: number = 1000) => 
  new Promise(resolve => setTimeout(resolve, ms));