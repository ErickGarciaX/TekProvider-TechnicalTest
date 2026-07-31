import { z } from 'zod';
import type { DynamicFieldSchema } from '../../design-system/DynamicForm';

export const loginFormSchema = z.object({
  username: z.string().min(1, 'Required'),
  password: z.string().min(1, 'Required'),
});

export type LoginFormValues = z.infer<typeof loginFormSchema>;

export const loginFormFields: DynamicFieldSchema<LoginFormValues>[] = [
  { name: 'username', label: 'Username', type: 'text', required: true },
  { name: 'password', label: 'Password', type: 'password', required: true },
];

export const registerFormSchema = z.object({
  username: z.string().min(1, 'Required'),
  password: z.string().min(8, 'At least 8 characters'),
});

export type RegisterFormValues = z.infer<typeof registerFormSchema>;

export const registerFormFields: DynamicFieldSchema<RegisterFormValues>[] = [
  { name: 'username', label: 'Username', type: 'text', required: true },
  { name: 'password', label: 'Password', type: 'password', required: true },
];
