import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getComments, addComment } from "../api/comments";

export const useComments = (taskId) => {
  return useQuery({
    queryKey: ["comments", taskId],
    queryFn: () => getComments(taskId),
    enabled: !!taskId,
  });
};

export const useAddComment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ taskId, data }) => addComment(taskId, data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ["comments", data.taskId] });
      queryClient.invalidateQueries({ queryKey: ["timeline", data.taskId] });
    },
  });
};
