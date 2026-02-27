import client from "./client";

export const getProjects = async () => {
  const { data } = await client.get("/projects");
  return data;
};

export const getProject = async (projectId) => {
  const { data } = await client.get(`/projects/${projectId}`);
  return data;
};

export const createProject = async (payload) => {
  const { data } = await client.post("/projects", {
    name: payload.name,
    description: payload.description || null,
  });
  return data;
};
