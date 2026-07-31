import { Badge } from '@mantine/core';
import type { CustomerStatus } from '../types/customer';

const STATUS_COLORS: Record<CustomerStatus, string> = {
  Active: 'green',
  Inactive: 'gray',
  Suspended: 'orange',
};

interface StatusBadgeProps {
  status: CustomerStatus;
  onClick?: () => void;
}

export function StatusBadge({ status, onClick }: StatusBadgeProps) {
  return (
    <Badge
      component={onClick ? 'button' : 'div'}
      onClick={onClick}
      color={STATUS_COLORS[status]}
      style={onClick ? { cursor: 'pointer' } : undefined}
    >
      {status}
    </Badge>
  );
}
