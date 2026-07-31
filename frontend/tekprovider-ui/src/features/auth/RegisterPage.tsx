import { Alert, Anchor, Center, Paper, Stack, Text, Title } from '@mantine/core';
import { useNavigate, Link } from 'react-router-dom';
import { DynamicForm } from '../../design-system/DynamicForm';
import { ApiError } from '../../types/problemDetails';
import { registerFormFields, registerFormSchema, type RegisterFormValues } from './authFormSchema';
import { useRegister } from './hooks/useRegister';

export function RegisterPage() {
  const navigate = useNavigate();
  const register = useRegister();

  const handleSubmit = (values: RegisterFormValues) => {
    register.mutate(values, { onSuccess: () => navigate('/customers') });
  };

  return (
    <Center mih="100vh">
      <Paper withBorder shadow="md" p="xl" w={380}>
        <Stack>
          <Title order={2}>Create account</Title>
          <Text c="dimmed" size="sm">
            Password must be at least 8 characters
          </Text>

          {register.isError && (
            <Alert color="red">
              {register.error instanceof ApiError ? register.error.message : 'Registration failed'}
            </Alert>
          )}

          <DynamicForm
            fields={registerFormFields}
            schema={registerFormSchema}
            defaultValues={{ username: '', password: '' }}
            submitLabel="Register"
            onSubmit={handleSubmit}
            isSubmitting={register.isPending}
          />

          <Text size="sm" ta="center">
            Already have an account? <Anchor component={Link} to="/login">Sign in</Anchor>
          </Text>
        </Stack>
      </Paper>
    </Center>
  );
}
