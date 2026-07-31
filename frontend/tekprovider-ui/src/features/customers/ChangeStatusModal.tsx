import { Button, Modal, Select, Stack } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import { useState } from 'react';
import { ApiError } from '../../types/problemDetails';
import { CUSTOMER_STATUS_TRANSITIONS, type Customer, type CustomerStatus } from '../../types/customer';
import { useChangeCustomerStatus } from './hooks/useChangeCustomerStatus';

interface ChangeStatusModalProps {
  opened: boolean;
  onClose: () => void;
  customer: Customer;
}

export function ChangeStatusModal({ opened, onClose, customer }: ChangeStatusModalProps) {
  const changeStatus = useChangeCustomerStatus();
  const availableStatuses = CUSTOMER_STATUS_TRANSITIONS[customer.status];
  const [newStatus, setNewStatus] = useState<CustomerStatus | null>(availableStatuses[0] ?? null);

  const handleConfirm = () => {
    if (!newStatus) return;

    changeStatus
      .mutateAsync({ id: customer.id, newStatus })
      .then(() => {
        notifications.show({ color: 'green', message: `Status changed to ${newStatus}` });
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
    <Modal opened={opened} onClose={onClose} title={`Change status — ${customer.name}`}>
      <Stack>
        <Select
          label="New status"
          data={availableStatuses}
          value={newStatus}
          onChange={(value) => setNewStatus(value as CustomerStatus)}
          allowDeselect={false}
        />
        <Button onClick={handleConfirm} loading={changeStatus.isPending} disabled={!newStatus} fullWidth>
          Confirm
        </Button>
      </Stack>
    </Modal>
  );
}
