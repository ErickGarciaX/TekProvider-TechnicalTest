import { useMutation } from '@tanstack/react-query';
import { authApi } from '../../../api/auth';
import { useAuthStore } from '../../../store/authStore';

export function useRegister() {
  const setSession = useAuthStore((state) => state.setSession);

  return useMutation({
    mutationFn: authApi.register,
    onSuccess: (response) => setSession(response.token, response.username),
  });
}
