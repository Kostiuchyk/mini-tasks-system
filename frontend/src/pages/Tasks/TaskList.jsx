import { useState, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  TablePagination,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Button,
  Box,
  Stack,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import DownloadIcon from "@mui/icons-material/Download";
import StatusChip from "../../components/StatusChip";
import PriorityChip from "../../components/PriorityChip";
import { LoadingState, ErrorState } from "../../components/StateIndicators";
import { useTasks, useCreateTask } from "../../hooks/useTasks";
import { useUsers } from "../../hooks/useUsers";
import { useProject } from "../../hooks/useProjects";
import { TASK_STATUS_OPTIONS, TASK_PRIORITY_OPTIONS } from "../../constants";
import { useAuth } from "../../context/AuthContext";
import { getTasks } from "../../api/tasks";
import { exportToCsv } from "../../utils/exportToCsv";

const TaskList = () => {
  const { projectId } = useParams();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [statusFilter, setStatusFilter] = useState("");
  const [assigneeFilter, setAssigneeFilter] = useState("");
  const [search, setSearch] = useState("");
  const [searchDebounced, setSearchDebounced] = useState("");

  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ title: "", description: "", priority: "Medium", assigneeId: "" });

  const filters = useMemo(
    () => ({
      status: statusFilter || undefined,
      assigneeId: assigneeFilter || undefined,
      search: searchDebounced || undefined,
      page: page + 1,
      pageSize,
    }),
    [statusFilter, assigneeFilter, searchDebounced, page, pageSize]
  );

  const { data: project } = useProject(projectId);
  const { data: taskData, isLoading, error } = useTasks(projectId, filters);
  const { data: users } = useUsers();
  const createTask = useCreateTask();

  // Simple debounce for search
  const handleSearchChange = (e) => {
    setSearch(e.target.value);
    clearTimeout(window.__searchTimeout);
    window.__searchTimeout = setTimeout(() => {
      setSearchDebounced(e.target.value);
      setPage(0);
    }, 300);
  };

  const handleCreate = () => {
    if (!form.title.trim()) return;
    createTask.mutate(
      { ...form, projectId, assigneeId: form.assigneeId || null },
      {
        onSuccess: () => {
          setOpen(false);
          setForm({ title: "", description: "", priority: "Medium", assigneeId: "" });
        },
      }
    );
  };

  const getUserName = (id) => {
    const user = users?.find((u) => u.id === id);
    return user ? user.fullName : "Unassigned";
  };

  const [exporting, setExporting] = useState(false);

  const handleExport = async () => {
    setExporting(true);
    try {
      const result = await getTasks(projectId, {
        status: statusFilter || undefined,
        assigneeId: assigneeFilter || undefined,
        search: searchDebounced || undefined,
        page: 1,
        pageSize: 10000,
      });
      exportToCsv(
        ["#", "Title", "Status", "Priority", "Assignee", "Created", "Updated"],
        result.data.map((t) => [
          `#${t.number}`,
          t.title,
          t.status,
          t.priority,
          getUserName(t.assigneeId),
          new Date(t.createdAt).toLocaleDateString(),
          new Date(t.updatedAt).toLocaleDateString(),
        ]),
        `tasks-${projectId}`
      );
    } finally {
      setExporting(false);
    }
  };

  if (isLoading) return <LoadingState />;
  if (error) return <ErrorState message={error.message} />;

  return (
    <>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h4">{project?.name || "Tasks"}</Typography>
        <Box sx={{ display: "flex", gap: 1 }}>
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            disabled={exporting || !taskData?.total}
            onClick={handleExport}
          >
            {exporting ? "Exporting…" : "Export CSV"}
          </Button>
          {isAdmin && (
            <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>
              New Task
            </Button>
          )}
        </Box>
      </Box>

      {/* Filters */}
      <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        <TextField
          label="Search"
          size="small"
          value={search}
          onChange={handleSearchChange}
          sx={{ minWidth: 200 }}
        />
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>Status</InputLabel>
          <Select
            value={statusFilter}
            label="Status"
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(0);
            }}
          >
            <MenuItem value="">All</MenuItem>
            {TASK_STATUS_OPTIONS.map((opt) => (
              <MenuItem key={opt.value} value={opt.value}>
                {opt.label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        <FormControl size="small" sx={{ minWidth: 140 }}>
          <InputLabel>Assignee</InputLabel>
          <Select
            value={assigneeFilter}
            label="Assignee"
            onChange={(e) => {
              setAssigneeFilter(e.target.value);
              setPage(0);
            }}
          >
            <MenuItem value="">All</MenuItem>
            {users?.map((u) => (
              <MenuItem key={u.id} value={u.id}>
                {u.fullName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Stack>

      {/* Table */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>#</TableCell>
              <TableCell>Title</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Priority</TableCell>
              <TableCell>Assignee</TableCell>
              <TableCell>Created ↓</TableCell>
              <TableCell>Updated</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {taskData?.data.map((task) => (
              <TableRow
                key={task.id}
                hover
                sx={{ cursor: "pointer" }}
                onClick={() => navigate(`/projects/${projectId}/tasks/${task.id}`)}
              >
                <TableCell>#{task.number}</TableCell>
                <TableCell>{task.title}</TableCell>
                <TableCell>
                  <StatusChip status={task.status} />
                </TableCell>
                <TableCell>
                  <PriorityChip priority={task.priority} />
                </TableCell>
                <TableCell>{getUserName(task.assigneeId)}</TableCell>
                <TableCell>{new Date(task.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>{new Date(task.updatedAt).toLocaleDateString()}</TableCell>
              </TableRow>
            ))}
            {taskData?.data.length === 0 && (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  No tasks found
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={taskData?.total || 0}
          page={page}
          onPageChange={(_, newPage) => setPage(newPage)}
          rowsPerPage={pageSize}
          onRowsPerPageChange={(e) => {
            setPageSize(parseInt(e.target.value, 10));
            setPage(0);
          }}
          rowsPerPageOptions={[5, 10, 25]}
        />
      </TableContainer>

      {/* Create Dialog */}
      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create Task</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            label="Title"
            fullWidth
            margin="normal"
            value={form.title}
            onChange={(e) => setForm({ ...form, title: e.target.value })}
          />
          <TextField
            label="Description"
            fullWidth
            margin="normal"
            multiline
            rows={3}
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
          />
          <FormControl fullWidth margin="normal">
            <InputLabel>Priority</InputLabel>
            <Select
              value={form.priority}
              label="Priority"
              onChange={(e) => setForm({ ...form, priority: e.target.value })}
            >
              {TASK_PRIORITY_OPTIONS.map((opt) => (
                <MenuItem key={opt.value} value={opt.value}>
                  {opt.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth margin="normal">
            <InputLabel>Assignee</InputLabel>
            <Select
              value={form.assigneeId}
              label="Assignee"
              onChange={(e) => setForm({ ...form, assigneeId: e.target.value })}
            >
              <MenuItem value="">Unassigned</MenuItem>
              {users?.map((u) => (
                <MenuItem key={u.id} value={u.id}>
                  {u.fullName}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={createTask.isPending}>
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default TaskList;
