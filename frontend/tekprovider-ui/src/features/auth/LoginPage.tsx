import { Alert, Anchor, Center, Paper, Stack, Text, Title } from '@mantine/core';
import { useNavigate, Link } from 'react-router-dom';
import { DynamicForm } from '../../design-system/DynamicForm';
import { ApiError } from '../../types/problemDetails';
import { loginFormFields, loginFormSchema, type LoginFormValues } from './authFormSchema';
import { useLogin } from './hooks/useLogin';

export function LoginPage() {
  const navigate = useNavigate();
  const login = useLogin();

  const handleSubmit = (values: LoginFormValues) => {
    login.mutate(values, { onSuccess: () => navigate('/customers') });
  };

  return (
    <Center mih="100vh">
      <Paper withBorder shadow="md" p="xl" w={380}>
        <Stack>
          <Title order={2}>TekProvider</Title>
          <Text c="dimmed" size="sm">
            Sign in to manage customers
          </Text>

          {login.isError && (
            <Alert color="red">
              {login.error instanceof ApiError ? login.error.message : 'Login failed'}
            </Alert>
          )}

          <DynamicForm
            fields={loginFormFields}
            schema={loginFormSchema}
            defaultValues={{ username: '', password: '' }}
            submitLabel="Sign in"
            onSubmit={handleSubmit}
            isSubmitting={login.isPending}
          />

          <Text size="sm" ta="center">
            No account? <Anchor component={Link} to="/register">Register</Anchor>
          </Text>
        </Stack>
      </Paper>
    </Center>
  );
}
