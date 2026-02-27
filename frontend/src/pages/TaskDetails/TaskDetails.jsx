import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Typography,
  Paper,
  Box,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  TextField,
  Button,
  Divider,
  List,
  ListItem,
  ListItemText,
  Stack,
  Card,
  CardContent,
} from "@mui/material";
import TimelineIcon from "@mui/icons-material/Timeline";
import StatusChip from "../../components/StatusChip";
import PriorityChip from "../../components/PriorityChip";
import { LoadingState, ErrorState } from "../../components/StateIndicators";
import { useTask, useUpdateTask } from "../../hooks/useTasks";
import { useComments, useAddComment } from "../../hooks/useComments";
import { useUsers } from "../../hooks/useUsers";
import { TASK_STATUS_OPTIONS, TASK_PRIORITY_OPTIONS } from "../../constants";
import { useAuth } from "../../context/AuthContext";

const TaskDetails = () => {
  const { projectId, taskId } = useParams();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();

  const { data: task, isLoading, error } = useTask(taskId);
  const { data: comments, isLoading: commentsLoading } = useComments(taskId);
  const { data: users } = useUsers();
  const updateTask = useUpdateTask();
  const addComment = useAddComment();

  const [commentText, setCommentText] = useState("");

  const handleUpdate = (field, value) => {
    updateTask.mutate({ taskId, data: { [field]: value } });
  };

  const handleAddComment = () => {
    if (!commentText.trim()) return;
    addComment.mutate(
      { taskId, data: { content: commentText } },
      {
        onSuccess: () => setCommentText(""),
      }
    );
  };

  const getUserName = (id) => {
    const user = users?.find((u) => u.id === id);
    return user ? user.fullName : "Unknown";
  };

  if (isLoading) return <LoadingState />;
  if (error) return <ErrorState message={error.message} />;

  return (
    <>
      {/* Task Details */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
          <Typography variant="h5">{task.title}</Typography>
          <Button
            variant="outlined"
            startIcon={<TimelineIcon />}
            onClick={() => navigate(`/projects/${projectId}/tasks/${taskId}/timeline`)}
          >
            Timeline
          </Button>
        </Box>

        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          {task.description}
        </Typography>

        <Stack direction="row" spacing={3}>
          {isAdmin ? (
            <FormControl size="small" sx={{ minWidth: 140 }}>
              <InputLabel>Status</InputLabel>
              <Select
                value={task.status}
                label="Status"
                onChange={(e) => handleUpdate("status", e.target.value)}
              >
                {TASK_STATUS_OPTIONS.map((opt) => (
                  <MenuItem key={opt.value} value={opt.value}>
                    {opt.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : (
            <Box><Typography variant="caption" color="text.secondary">Status</Typography><Box><StatusChip status={task.status} /></Box></Box>
          )}

          {isAdmin ? (
            <FormControl size="small" sx={{ minWidth: 140 }}>
              <InputLabel>Priority</InputLabel>
              <Select
                value={task.priority}
                label="Priority"
                onChange={(e) => handleUpdate("priority", e.target.value)}
              >
                {TASK_PRIORITY_OPTIONS.map((opt) => (
                  <MenuItem key={opt.value} value={opt.value}>
                    {opt.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : (
            <Box><Typography variant="caption" color="text.secondary">Priority</Typography><Box><PriorityChip priority={task.priority} /></Box></Box>
          )}

          {isAdmin ? (
            <FormControl size="small" sx={{ minWidth: 160 }}>
              <InputLabel>Assignee</InputLabel>
              <Select
                value={task.assigneeId || ""}
                label="Assignee"
                onChange={(e) => handleUpdate("assigneeId", e.target.value || null)}
              >
                <MenuItem value="">Unassigned</MenuItem>
                {users?.map((u) => (
                  <MenuItem key={u.id} value={u.id}>
                    {u.fullName}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          ) : (
            <Box><Typography variant="caption" color="text.secondary">Assignee</Typography><Typography variant="body2">{getUserName(task.assigneeId)}</Typography></Box>
          )}
        </Stack>

        <Box sx={{ mt: 2 }}>
          <Typography variant="caption" color="text.secondary">
            Created: {new Date(task.createdAt).toLocaleString()} &nbsp;|&nbsp; Updated:{" "}
            {new Date(task.updatedAt).toLocaleString()}
          </Typography>
        </Box>
      </Paper>

      {/* Comments */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Comments ({comments?.length || 0})
        </Typography>
        <Divider sx={{ mb: 2 }} />

        {commentsLoading ? (
          <LoadingState />
        ) : (
          <List>
            {comments?.length === 0 && (
              <Typography variant="body2" color="text.secondary" sx={{ py: 2 }}>
                No comments yet
              </Typography>
            )}
            {comments?.map((comment) => (
              <Card key={comment.id} variant="outlined" sx={{ mb: 1 }}>
                <CardContent sx={{ py: 1, "&:last-child": { pb: 1 } }}>
                  <Stack direction="row" justifyContent="space-between" alignItems="center">
                    <Typography variant="subtitle2">{getUserName(comment.authorId)}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {new Date(comment.createdAt).toLocaleString()}
                    </Typography>
                  </Stack>
                  <Typography variant="body2" sx={{ mt: 0.5 }}>
                    {comment.text}
                  </Typography>
                </CardContent>
              </Card>
            ))}
          </List>
        )}

        <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
            <TextField
              fullWidth
              size="small"
              placeholder="Add a comment..."
              value={commentText}
              onChange={(e) => setCommentText(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter" && !e.shiftKey) {
                  e.preventDefault();
                  handleAddComment();
                }
              }}
            />
            <Button variant="contained" onClick={handleAddComment} disabled={addComment.isPending}>
              Add
            </Button>
          </Box>
      </Paper>
    </>
  );
};

export default TaskDetails;
