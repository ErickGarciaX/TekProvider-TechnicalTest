import { useQuery } from '@tanstack/react-query';
import { customersApi } from '../../../api/customers';

export function useCustomer(id: string | undefined) {
  return useQuery({
    queryKey: ['customer', id],
    queryFn: () => customersApi.getById(id as string),
    enabled: Boolean(id),
  });
}
