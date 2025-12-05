import api from '../../services/api';

export interface BusFleetItem {
  busId: number;
  busName: string;
  registrationNo: string;
  busType: number;
  totalSeats: number;
  status: number;
  amenities: number;
  isActive: boolean;
  driverName?: string;
  driverContact?: string;
  vendorId: number;
  vendorName: string;
  seatLayoutTemplateId: number;
  createdAt: string;
  createdBy: string;
  updatedBy?: string;
}

export interface CreateBusRequest {
  busName: string;
  busType: number;
  totalSeats: number;
  registrationNo: string;
  amenities: number;
  seatLayoutTemplateId: number;
  driverName?: string;
  driverContact?: string;
  registrationCertificate: File;
}

export interface UpdateBusRequest {
  busName: string;
  busType: number;
  totalSeats: number;
  amenities: number;
  driverName?: string;
  driverContact?: string;
}

export interface BusPhoto {
  busPhotoId: number;
  busId: number;
  imagePath: string;
  caption?: string;
  isPrimary: boolean;
}

export interface SeatLayoutTemplate {
  seatLayoutTemplateId: number;
  templateName: string;
  totalSeats: number;
  busType: number;
  description?: string;
}

export const busFleetAPI = {
  getBusFleet: async (params: {
    pageNumber?: number;
    pageSize?: number;
    status?: number;
    search?: string;
  }) => {
    const response = await api.get('/bus/my-buses', { params });
    return response.data;
  },

  createBus: async (data: CreateBusRequest) => {
    const formData = new FormData();
    formData.append('busName', data.busName);
    formData.append('busType', data.busType.toString());
    formData.append('totalSeats', data.totalSeats.toString());
    formData.append('registrationNo', data.registrationNo);
    formData.append('amenities', data.amenities.toString());
    formData.append('seatLayoutTemplateId', data.seatLayoutTemplateId.toString());
    if (data.driverName) formData.append('driverName', data.driverName);
    if (data.driverContact) formData.append('driverContact', data.driverContact);
    formData.append('registrationCertificate', data.registrationCertificate);

    const response = await api.post('/bus', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  updateBus: async (busId: number, data: UpdateBusRequest) => {
    const formData = new FormData();
    formData.append('busName', data.busName);
    formData.append('busType', data.busType.toString());
    formData.append('totalSeats', data.totalSeats.toString());
    formData.append('amenities', data.amenities.toString());
    if (data.driverName) formData.append('driverName', data.driverName);
    if (data.driverContact) formData.append('driverContact', data.driverContact);

    const response = await api.put(`/bus/${busId}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  deleteBus: async (busId: number) => {
    const response = await api.delete(`/bus/${busId}`);
    return response.data;
  },

  getBusDetails: async (busId: number) => {
    const response = await api.get(`/bus/${busId}`);
    return response.data;
  },

  changeBusStatus: async (busId: number, action: 'activate' | 'deactivate' | 'maintenance') => {
    const response = await api.put(`/bus/${busId}/${action}`);
    return response.data;
  },

  getBusPhotos: async (busId: number) => {
    const response = await api.get(`/busphoto/bus/${busId}`);
    return response.data;
  },

  addBusPhoto: async (busId: number, photo: File, caption?: string) => {
    const formData = new FormData();
    formData.append('busId', busId.toString());
    formData.append('photo', photo);
    if (caption) formData.append('caption', caption);

    const response = await api.post('/busphoto', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  deleteBusPhoto: async (photoId: number) => {
    const response = await api.delete(`/busphoto/${photoId}`);
    return response.data;
  },

  getSeatLayoutTemplates: async () => {
    const response = await api.get('/admin/AdminSeatLayout/seat-layout-templates');
    return response.data;
  },

  getSeatLayoutDetails: async (templateId: number) => {
    const response = await api.get(`/admin/AdminSeatLayout/seat-layout-templates/${templateId}`);
    return response.data;
  },

  getBusSeatLayout: async (busId: number) => {
    const response = await api.get(`/bus/${busId}/seat-layout`);
    return response.data;
  },

  applySeatLayoutTemplate: async (busId: number, templateId: number) => {
    const response = await api.post(`/bus/${busId}/apply-template/${templateId}`);
    return response.data;
  }
};