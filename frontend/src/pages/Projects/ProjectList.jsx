import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Typography,
  Card,
  CardContent,
  CardActionArea,
  Grid,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Box,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import DownloadIcon from "@mui/icons-material/Download";
import { useProjects, useCreateProject } from "../../hooks/useProjects";
import { LoadingState, ErrorState } from "../../components/StateIndicators";
import { useAuth } from "../../context/AuthContext";
import { exportToCsv } from "../../utils/exportToCsv";

const ProjectList = () => {
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const { data: projects, isLoading, error } = useProjects();
  const createProject = useCreateProject();

  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: "", description: "" });

  const handleCreate = () => {
    if (!form.name.trim()) return;
    createProject.mutate(form, {
      onSuccess: () => {
        setOpen(false);
        setForm({ name: "", description: "" });
      },
    });
  };

  if (isLoading) return <LoadingState />;
  if (error) return <ErrorState message={error.message} />;

  return (
    <>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
        <Typography variant="h4">Projects</Typography>
        <Box sx={{ display: "flex", gap: 1 }}>
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            disabled={!projects?.length}
            onClick={() =>
              exportToCsv(
                ["Name", "Description", "Created"],
                projects.map((p) => [
                  p.name,
                  p.description,
                  new Date(p.createdAt).toLocaleDateString(),
                ]),
                "projects"
              )
            }
          >
            Export CSV
          </Button>
          {isAdmin && (
            <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpen(true)}>
              New Project
            </Button>
          )}
        </Box>
      </Box>

      <Grid container spacing={2}>
        {projects.map((project) => (
          <Grid size={{ xs: 12, sm: 6, md: 4 }} key={project.id}>
            <Card>
              <CardActionArea onClick={() => navigate(`/projects/${project.id}/tasks`)}>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {project.name}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {project.description}
                  </Typography>
                  <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: "block" }}>
                    Created: {new Date(project.createdAt).toLocaleDateString()}
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create Project</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            label="Name"
            fullWidth
            margin="normal"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
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
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={createProject.isPending}>
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default ProjectList;
