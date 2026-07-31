import { useAuthStore } from '../store/authStore';
import { ApiError, type ProblemDetails } from '../types/problemDetails';

const BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

async function request<TResponse>(path: string, init?: RequestInit): Promise<TResponse> {
  const token = useAuthStore.getState().token;

  const response = await fetch(`${BASE_URL}${path}`, {
    ...init,
    headers: {
      ...(init?.body ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
  });

  if (response.status === 401) {
    useAuthStore.getState().clearSession();
  }

  if (!response.ok) {
    const problemDetails: ProblemDetails = await response
      .json()
      .catch(() => ({ title: response.statusText, status: response.status }));
    throw new ApiError(problemDetails);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return (await response.json()) as TResponse;
}

export const httpClient = {
  get: <TResponse>(path: string) => request<TResponse>(path, { method: 'GET' }),
  post: <TResponse>(path: string, body?: unknown) =>
    request<TResponse>(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined }),
  put: <TResponse>(path: string, body?: unknown) =>
    request<TResponse>(path, { method: 'PUT', body: body ? JSON.stringify(body) : undefined }),
  patch: <TResponse>(path: string, body?: unknown) =>
    request<TResponse>(path, { method: 'PATCH', body: body ? JSON.stringify(body) : undefined }),
};
