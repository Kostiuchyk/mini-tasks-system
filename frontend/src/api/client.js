import axios from "axios";

const AUTH_KEY = "mini_tasks_auth";

const client = axios.create({
  baseURL: "http://localhost:8080/api",
});

client.interceptors.request.use((config) => {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    if (raw) {
      const auth = JSON.parse(raw);
      if (auth.accessToken) {
        config.headers.Authorization = `Bearer ${auth.accessToken}`;
      }
    }
  } catch {
    // ignore
  }
  return config;
});

client.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const raw = localStorage.getItem(AUTH_KEY);
        if (raw) {
          const auth = JSON.parse(raw);
          if (auth.refreshToken) {
            const { data } = await axios.post("https://localhost:7288/api/auth/refresh", {
              refreshToken: auth.refreshToken,
            });

            localStorage.setItem(
              AUTH_KEY,
              JSON.stringify({
                accessToken: data.accessToken,
                refreshToken: data.refreshToken,
                user: data.user,
              })
            );

            originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
            return client(originalRequest);
          }
        }
      } catch {
        // Refresh failed — clear auth and redirect
      }

      localStorage.removeItem(AUTH_KEY);
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);

export default client;
