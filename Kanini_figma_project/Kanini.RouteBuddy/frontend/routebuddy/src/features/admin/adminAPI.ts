import api from '../../services/api';

export interface AdminVendor {
  vendorId: number;
  agencyName: string;
  ownerName: string;
  status: number;
  isActive: boolean;
  createdOn: string;
  email: string;
  phone: string;
  totalBuses: number;
}

export interface VendorDocument {
  documentId: number;
  vendorId: number;
  documentFile: number;
  documentPath: string;
  issueDate?: string;
  expiryDate?: string;
  uploadedAt: string;
  isVerified: boolean;
  status: number;
}

export interface VendorApproval {
  vendorId: number;
  userId: number;
  agencyName: string;
  ownerName: string;
  businessLicenseNumber: string;
  officeAddress: string;
  fleetSize: number;
  taxRegistrationNumber?: string;
  status: number;
  statusText: string;
  isActive: boolean;
  createdOn: string;
  email: string;
  phone: string;
  documents: VendorDocument[];
}

export interface VendorRejection {
  rejectionReason: string;
}

export const adminAPI = {
  getPendingVendors: async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get(`/admin/vendors/pending?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    return response.data;
  },

  getAllVendors: async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get(`/admin/vendors?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    return response.data;
  },

  filterVendors: async (searchName?: string, isActive?: boolean, status?: number) => {
    const params = new URLSearchParams();
    if (searchName) params.append('searchName', searchName);
    if (isActive !== undefined) params.append('isActive', isActive.toString());
    if (status !== undefined) params.append('status', status.toString());
    
    const response = await api.get(`/admin/vendors/filter?${params.toString()}`);
    return response.data;
  },

  getVendorById: async (vendorId: number) => {
    const response = await api.get(`/admin/vendors/${vendorId}`);
    return response.data;
  },

  approveVendor: async (vendorId: number) => {
    const response = await api.patch(`/admin/vendors/approve/${vendorId}`);
    return response.data;
  },

  rejectVendor: async (vendorId: number, rejectionData: VendorRejection) => {
    const response = await api.patch(`/admin/vendors/reject/${vendorId}`, rejectionData);
    return response.data;
  },

  deactivateVendor: async (vendorId: number, reasonData: VendorRejection) => {
    const response = await api.put(`/admin/vendors/deactivate/${vendorId}`, reasonData);
    return response.data;
  },

  reactivateVendor: async (vendorId: number, reasonData: VendorRejection) => {
    const response = await api.put(`/admin/vendors/reactivate/${vendorId}`, reasonData);
    return response.data;
  },

  getVendorForApproval: async (vendorId: number) => {
    const response = await api.get(`/admin/vendors/${vendorId}/approval`);
    return response.data;
  },
};