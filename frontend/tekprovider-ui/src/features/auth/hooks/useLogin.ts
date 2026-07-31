import { useMutation } from '@tanstack/react-query';
import { authApi } from '../../../api/auth';
import { useAuthStore } from '../../../store/authStore';

export function useLogin() {
  const setSession = useAuthStore((state) => state.setSession);

  return useMutation({
    mutationFn: authApi.login,
    onSuccess: (response) => setSession(response.token, response.username),
  });
}
