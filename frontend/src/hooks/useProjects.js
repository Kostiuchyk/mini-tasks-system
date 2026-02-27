import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getProjects, getProject, createProject } from "../api/projects";

export const useProjects = () => {
  return useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });
};

export const useProject = (projectId) => {
  return useQuery({
    queryKey: ["projects", projectId],
    queryFn: () => getProject(projectId),
    enabled: !!projectId,
  });
};

export const useCreateProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createProject,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
    },
  });
};
