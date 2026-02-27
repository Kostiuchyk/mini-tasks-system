import client from "./client";

export const getComments = async (taskId) => {
  const { data } = await client.get(`/tasks/${taskId}/comments`);
  return data;
};

export const addComment = async (taskId, payload) => {
  const { data } = await client.post(`/tasks/${taskId}/comments`, {
    text: payload.content,
  });
  return data;
};
