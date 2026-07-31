import { useMutation, useQueryClient } from '@tanstack/react-query';
import { customersApi } from '../../../api/customers';

export function useChangeCustomerStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: customersApi.changeStatus,
    onSuccess: (customer) => {
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      queryClient.setQueryData(['customer', customer.id], customer);
    },
  });
}
