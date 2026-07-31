import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { customersApi, type GetCustomersParams } from '../../../api/customers';

export const customersQueryKey = (params: GetCustomersParams) => ['customers', params] as const;

export function useCustomers(params: GetCustomersParams) {
  return useQuery({
    queryKey: customersQueryKey(params),
    queryFn: () => customersApi.getPaged(params),
    placeholderData: keepPreviousData,
  });
}
