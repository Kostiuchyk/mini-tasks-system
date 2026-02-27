import client from "./client";

export const getUsers = async () => {
  const { data } = await client.get("/users");
  return data;
};
