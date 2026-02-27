import { useQuery } from "@tanstack/react-query";
import { getTimeline } from "../api/timeline";

export const useTimeline = (taskId) => {
  return useQuery({
    queryKey: ["timeline", taskId],
    queryFn: () => getTimeline(taskId),
    enabled: !!taskId,
  });
};
