import { z } from 'zod';
import type { DynamicFieldSchema } from '../../design-system/DynamicForm';

export const customerFormSchema = z.object({
  name: z.string().min(1, 'Required').max(200),
  taxId: z.string().min(1, 'Required').max(20),
  email: z.string().min(1, 'Required').email('Invalid email'),
  phone: z.string().max(20).optional().or(z.literal('')),
});

export type CustomerFormValues = z.infer<typeof customerFormSchema>;

export const customerFormFields: DynamicFieldSchema<CustomerFormValues>[] = [
  { name: 'name', label: 'Name', type: 'text', required: true },
  { name: 'taxId', label: 'Tax ID', type: 'text', required: true },
  { name: 'email', label: 'Email', type: 'email', required: true },
  { name: 'phone', label: 'Phone', type: 'tel' },
];
