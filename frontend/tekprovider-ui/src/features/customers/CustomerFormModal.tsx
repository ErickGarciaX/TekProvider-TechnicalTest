import { Modal } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import { DynamicForm } from '../../design-system/DynamicForm';
import { ApiError } from '../../types/problemDetails';
import type { Customer } from '../../types/customer';
import { customerFormFields, customerFormSchema, type CustomerFormValues } from './customerFormSchema';
import { useCreateCustomer } from './hooks/useCreateCustomer';
import { useUpdateCustomer } from './hooks/useUpdateCustomer';

interface CustomerFormModalProps {
  opened: boolean;
  onClose: () => void;
  customer?: Customer;
}

export function CustomerFormModal({ opened, onClose, customer }: CustomerFormModalProps) {
  const createCustomer = useCreateCustomer();
  const updateCustomer = useUpdateCustomer();
  const isEditing = Boolean(customer);
  const isSubmitting = createCustomer.isPending || updateCustomer.isPending;

  const handleSubmit = (values: CustomerFormValues) => {
    const input = { ...values, phone: values.phone || null };

    const mutation = customer
      ? updateCustomer.mutateAsync({ id: customer.id, rowVersion: customer.rowVersion, ...input })
      : createCustomer.mutateAsync(input);

    mutation
      .then(() => {
        notifications.show({
          color: 'green',
          message: isEditing ? 'Customer updated' : 'Customer created',
        });
        onClose();
      })
      .catch((error: unknown) => {
        notifications.show({
          color: 'red',
          message: error instanceof ApiError ? error.message : 'Something went wrong',
        });
      });
  };

  return (
    <Modal opened={opened} onClose={onClose} title={isEditing ? 'Edit customer' : 'New customer'}>
      <DynamicForm
        fields={customerFormFields}
        schema={customerFormSchema}
        defaultValues={{
          name: customer?.name ?? '',
          taxId: customer?.taxId ?? '',
          email: customer?.email ?? '',
          phone: customer?.phone ?? '',
        }}
        submitLabel={isEditing ? 'Save changes' : 'Create'}
        onSubmit={handleSubmit}
        isSubmitting={isSubmitting}
      />
    </Modal>
  );
}
