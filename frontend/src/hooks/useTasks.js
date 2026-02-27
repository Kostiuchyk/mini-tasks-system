import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getTasks, getTask, createTask, updateTask } from "../api/tasks";

export const useTasks = (projectId, filters = {}) => {
  return useQuery({
    queryKey: ["tasks", projectId, filters],
    queryFn: () => getTasks(projectId, filters),
    enabled: !!projectId,
  });
};

export const useTask = (taskId) => {
  return useQuery({
    queryKey: ["task", taskId],
    queryFn: () => getTask(taskId),
    enabled: !!taskId,
  });
};

export const useCreateTask = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createTask,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["tasks", data.projectId] });
    },
  });
};

export const useUpdateTask = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ taskId, data }) => updateTask(taskId, data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["tasks", data.projectId] });
      queryClient.invalidateQueries({ queryKey: ["task", data.id] });
      queryClient.invalidateQueries({ queryKey: ["timeline", data.id] });
    },
  });
};
