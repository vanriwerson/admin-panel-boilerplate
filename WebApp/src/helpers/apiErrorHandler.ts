import axios from 'axios';

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    return (
      error.response?.data?.error ||
      error.response?.data?.message ||
      error.message ||
      'Erro desconhecido no servidor.'
    );
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'Erro inesperado.';
}
