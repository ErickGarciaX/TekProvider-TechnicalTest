import { httpClient } from './httpClient';
import type {
  ChangeCustomerStatusInput,
  Customer,
  CreateCustomerInput,
  PagedResult,
  UpdateCustomerInput,
} from '../types/customer';

export interface GetCustomersParams {
  search?: string;
  page: number;
  pageSize: number;
}

export const customersApi = {
  getPaged: ({ search, page, pageSize }: GetCustomersParams) => {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    if (search) {
      params.set('search', search);
    }
    return httpClient.get<PagedResult<Customer>>(`/api/customers?${params.toString()}`);
  },

  getById: (id: string) => httpClient.get<Customer>(`/api/customers/${id}`),

  create: (input: CreateCustomerInput) => httpClient.post<Customer>('/api/customers', input),

  update: ({ id, ...input }: UpdateCustomerInput) =>
    httpClient.put<Customer>(`/api/customers/${id}`, input),

  changeStatus: ({ id, newStatus }: ChangeCustomerStatusInput) =>
    httpClient.patch<Customer>(`/api/customers/${id}/status`, { newStatus }),
};
