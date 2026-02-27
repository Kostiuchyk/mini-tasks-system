import client from "./client";

export const getTasks = async (projectId, { status, assigneeId, search, page = 1, pageSize = 10 } = {}) => {
  const params = { projectId, page, pageSize };
  if (status) params.status = status;
  if (assigneeId) params.assigneeId = assigneeId;
  if (search) params.search = search;

  const { data } = await client.get("/tasks", { params });

  // Map backend shape to frontend shape
  return {
    data: data.items,
    total: data.totalCount,
    page: data.page,
    pageSize: data.pageSize,
    totalPages: data.totalPages,
  };
};

export const getTask = async (taskId) => {
  const { data } = await client.get(`/tasks/${taskId}`);
  return data;
};

export const createTask = async (payload) => {
  const { data } = await client.post("/tasks", {
    title: payload.title,
    description: payload.description || null,
    status: "New",
    priority: payload.priority || "Medium",
    projectId: payload.projectId,
    assigneeId: payload.assigneeId || null,
  });
  return data;
};

export const updateTask = async (taskId, updates) => {
  // Backend requires full payload — fetch current task and merge
  const { data: current } = await client.get(`/tasks/${taskId}`);

  const { data } = await client.put(`/tasks/${taskId}`, {
    title: updates.title ?? current.title,
    description: updates.description ?? current.description,
    status: updates.status ?? current.status,
    priority: updates.priority ?? current.priority,
    assigneeId: updates.assigneeId !== undefined ? updates.assigneeId : current.assigneeId,
  });
  return data;
};
