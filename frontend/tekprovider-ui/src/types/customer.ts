export type CustomerStatus = 'Active' | 'Inactive' | 'Suspended';

export interface Customer {
  id: string;
  name: string;
  taxId: string;
  email: string;
  phone: string | null;
  status: CustomerStatus;
  createdAt: string;
  rowVersion: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreateCustomerInput {
  name: string;
  taxId: string;
  email: string;
  phone?: string | null;
}

export interface UpdateCustomerInput extends CreateCustomerInput {
  id: string;
  rowVersion: number;
}

export interface ChangeCustomerStatusInput {
  id: string;
  newStatus: CustomerStatus;
}

// Mirrors the seeded CustomerStatusTransitions matrix in TekProvider.Infrastructure — keep in sync.
export const CUSTOMER_STATUS_TRANSITIONS: Record<CustomerStatus, CustomerStatus[]> = {
  Active: ['Inactive', 'Suspended'],
  Inactive: ['Active'],
  Suspended: ['Active'],
};
