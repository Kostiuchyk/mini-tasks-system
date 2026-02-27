import client from "./client";

export const getTimeline = async (taskId) => {
  const { data } = await client.get(`/timeline/${taskId}`);
  return data;
};
