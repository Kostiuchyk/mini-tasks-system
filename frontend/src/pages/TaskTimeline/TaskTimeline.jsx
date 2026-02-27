import { useParams } from "react-router-dom";
import { Typography, Paper, Box } from "@mui/material";
import {
  Timeline,
  TimelineItem,
  TimelineSeparator,
  TimelineConnector,
  TimelineContent,
  TimelineDot,
  TimelineOppositeContent,
} from "@mui/lab";
import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import SwapHorizIcon from "@mui/icons-material/SwapHoriz";
import PersonIcon from "@mui/icons-material/Person";
import CommentIcon from "@mui/icons-material/Comment";
import { LoadingState, ErrorState } from "../../components/StateIndicators";
import { useTimeline } from "../../hooks/useTimeline";
import { useTask } from "../../hooks/useTasks";
import { useUsers } from "../../hooks/useUsers";
import { AUDIT_ACTION_LABELS, TASK_STATUS, TASK_PRIORITY } from "../../constants";

const actionIcons = {
  created: <AddIcon />,
  status_changed: <SwapHorizIcon />,
  priority_changed: <EditIcon />,
  assignee_changed: <PersonIcon />,
  comment_added: <CommentIcon />,
};

const actionColors = {
  created: "success",
  status_changed: "primary",
  priority_changed: "warning",
  assignee_changed: "info",
  comment_added: "grey",
};

const TaskTimeline = () => {
  const { taskId } = useParams();
  const { data: task } = useTask(taskId);
  const { data: timeline, isLoading, error } = useTimeline(taskId);
  const { data: users } = useUsers();

  const getUserName = (id) => {
    const user = users?.find((u) => u.id === id);
    return user ? user.fullName : "Unknown";
  };

  const formatChange = (log) => {
    if (log.action === "created") {
      return `"${log.newValue}"`;
    }

    if (log.action === "comment_added") {
      return `"${log.newValue}"`;
    }

    if (log.action === "status_changed") {
      const fromLabel = TASK_STATUS[log.oldValue] || log.oldValue;
      const toLabel = TASK_STATUS[log.newValue] || log.newValue;
      return `${fromLabel} → ${toLabel}`;
    }

    if (log.action === "priority_changed") {
      const fromLabel = TASK_PRIORITY[log.oldValue] || log.oldValue;
      const toLabel = TASK_PRIORITY[log.newValue] || log.newValue;
      return `${fromLabel} → ${toLabel}`;
    }

    if (log.action === "assignee_changed") {
      const fromLabel = log.oldValue ? getUserName(log.oldValue) : "Unassigned";
      const toLabel = log.newValue ? getUserName(log.newValue) : "Unassigned";
      return `${fromLabel} → ${toLabel}`;
    }

    return "";
  };

  if (isLoading) return <LoadingState />;
  if (error) return <ErrorState message={error.message} />;

  return (
    <>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Timeline: {task?.title}
      </Typography>

      <Paper sx={{ p: 2 }}>
        {timeline?.length === 0 && (
          <Typography color="text.secondary" sx={{ py: 2, textAlign: "center" }}>
            No history yet
          </Typography>
        )}

        <Timeline position="alternate">
          {timeline?.map((log, index) => (
            <TimelineItem key={log.id}>
              <TimelineOppositeContent color="text.secondary" sx={{ flex: 0.3 }}>
                <Typography variant="caption">
                  {new Date(log.createdAt).toLocaleString()}
                </Typography>
              </TimelineOppositeContent>

              <TimelineSeparator>
                <TimelineDot color={actionColors[log.action] || "grey"}>
                  {actionIcons[log.action] || <EditIcon />}
                </TimelineDot>
                {index < timeline.length - 1 && <TimelineConnector />}
              </TimelineSeparator>

              <TimelineContent>
                <Typography variant="subtitle2">
                  {AUDIT_ACTION_LABELS[log.action] || log.action}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {formatChange(log)}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  by {getUserName(log.userId)}
                </Typography>
              </TimelineContent>
            </TimelineItem>
          ))}
        </Timeline>
      </Paper>
    </>
  );
};

export default TaskTimeline;
