import client from "./client";

const AUTH_KEY = "mini_tasks_auth";

const storeAuth = (data) => {
  localStorage.setItem(
    AUTH_KEY,
    JSON.stringify({
      accessToken: data.accessToken,
      refreshToken: data.refreshToken,
      user: data.user,
    })
  );
  return data.user;
};

export const login = async (email, password) => {
  const { data } = await client.post("/auth/login", { email, password });
  return storeAuth(data);
};

export const register = async (fullName, email, password) => {
  const { data } = await client.post("/auth/register", { fullName, email, password });
  return storeAuth(data);
};

export const logout = async () => {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    if (raw) {
      const auth = JSON.parse(raw);
      if (auth.refreshToken) {
        await client.post("/auth/logout", { refreshToken: auth.refreshToken });
      }
    }
  } catch {
    // ignore
  }
  localStorage.removeItem(AUTH_KEY);
};

export const getCurrentUser = () => {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    if (raw) {
      const auth = JSON.parse(raw);
      return auth.user || null;
    }
    return null;
  } catch {
    return null;
  }
};
