import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5209/api',
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest: any = error.config; // axios config with custom flag

    if (
      error.response?.status === 401 &&
      !originalRequest._retry // avoid infinite loop
    ) {
      const storedRefresh = localStorage.getItem('refreshToken');
      if (storedRefresh) {
        originalRequest._retry = true;
        try {
          const refreshResponse = await api.post('/auth/refresh', {
            refreshToken: storedRefresh,
          });
          const data = refreshResponse.data;

          // update local storage with new tokens
          localStorage.setItem('token', data.token);
          localStorage.setItem('refreshToken', data.refreshToken);

          // notify any listeners (AuthContext) that token changed
          window.dispatchEvent(new Event('tokenRefreshed'));

          // update header and retry original request
          originalRequest.headers.Authorization = `Bearer ${data.token}`;
          return api(originalRequest);
        } catch (refreshError) {
          // refresh also failed, fall through to logout
        }
      }

      // no refresh token or refresh failed: clear auth state
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('authUser');

      // tell the app that authentication has been lost so contexts can react
      window.dispatchEvent(new Event('logout'));
    }

    return Promise.reject(error);
  }
);

export default api;
