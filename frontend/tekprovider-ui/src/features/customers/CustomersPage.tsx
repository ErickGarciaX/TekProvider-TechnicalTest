import { Alert, Button, Container, Group, Pagination, Table, TextInput, Title } from '@mantine/core';
import { useState } from 'react';
import { StatusBadge } from '../../design-system/StatusBadge';
import { ApiError } from '../../types/problemDetails';
import type { Customer } from '../../types/customer';
import { useAuthStore } from '../../store/authStore';
import { CustomerFormModal } from './CustomerFormModal';
import { ChangeStatusModal } from './ChangeStatusModal';
import { useCustomers } from './hooks/useCustomers';

const PAGE_SIZE = 10;

export function CustomersPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [editingCustomer, setEditingCustomer] = useState<Customer | undefined>(undefined);
  const [isCreateModalOpen, setCreateModalOpen] = useState(false);
  const [statusChangeTarget, setStatusChangeTarget] = useState<Customer | undefined>(undefined);

  const clearSession = useAuthStore((state) => state.clearSession);
  const username = useAuthStore((state) => state.username);

  const { data, isLoading, isError, error } = useCustomers({ search, page, pageSize: PAGE_SIZE });

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 0;

  return (
    <Container size="lg" py="xl">
      <Group justify="space-between" mb="md">
        <Title order={2}>Customers</Title>
        <Group>
          <span>Signed in as {username}</span>
          <Button variant="subtle" onClick={clearSession}>
            Sign out
          </Button>
        </Group>
      </Group>

      <Group justify="space-between" mb="md">
        <TextInput
          placeholder="Search by name, tax ID or email"
          value={search}
          onChange={(event) => {
            setSearch(event.currentTarget.value);
            setPage(1);
          }}
          w={320}
        />
        <Button onClick={() => setCreateModalOpen(true)}>New customer</Button>
      </Group>

      {isError && (
        <Alert color="red" mb="md">
          {error instanceof ApiError ? error.message : 'Could not load customers'}
        </Alert>
      )}

      <Table striped highlightOnHover>
        <Table.Thead>
          <Table.Tr>
            <Table.Th>Name</Table.Th>
            <Table.Th>Tax ID</Table.Th>
            <Table.Th>Email</Table.Th>
            <Table.Th>Phone</Table.Th>
            <Table.Th>Status</Table.Th>
            <Table.Th />
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>
          {data?.items.map((customer) => (
            <Table.Tr key={customer.id}>
              <Table.Td>{customer.name}</Table.Td>
              <Table.Td>{customer.taxId}</Table.Td>
              <Table.Td>{customer.email}</Table.Td>
              <Table.Td>{customer.phone ?? '—'}</Table.Td>
              <Table.Td>
                <StatusBadge status={customer.status} onClick={() => setStatusChangeTarget(customer)} />
              </Table.Td>
              <Table.Td>
                <Button size="xs" variant="light" onClick={() => setEditingCustomer(customer)}>
                  Edit
                </Button>
              </Table.Td>
            </Table.Tr>
          ))}
        </Table.Tbody>
      </Table>

      {!isLoading && data?.items.length === 0 && <Alert mt="md">No customers found</Alert>}

      {totalPages > 1 && (
        <Group justify="center" mt="lg">
          <Pagination total={totalPages} value={page} onChange={setPage} />
        </Group>
      )}

      <CustomerFormModal opened={isCreateModalOpen} onClose={() => setCreateModalOpen(false)} />

      {editingCustomer && (
        <CustomerFormModal
          opened
          customer={editingCustomer}
          onClose={() => setEditingCustomer(undefined)}
        />
      )}

      {statusChangeTarget && (
        <ChangeStatusModal
          opened
          customer={statusChangeTarget}
          onClose={() => setStatusChangeTarget(undefined)}
        />
      )}
    </Container>
  );
}
