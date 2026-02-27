import { AppBar, Toolbar, Typography, Breadcrumbs, Link as MuiLink, Container, Box, Button, Chip, Stack } from "@mui/material";
import { Link, useLocation } from "react-router-dom";
import LogoutIcon from "@mui/icons-material/Logout";
import { useAuth } from "../context/AuthContext";
import { ROLES } from "../constants";

const Layout = ({ children }) => {
  const location = useLocation();
  const { user, logout, isAdmin } = useAuth();
  const pathParts = location.pathname.split("/").filter(Boolean);

  const buildBreadcrumbs = () => {
    const crumbs = [{ label: "Projects", path: "/" }];

    if (pathParts.length >= 2 && pathParts[0] === "projects") {
      crumbs.push({
        label: "Tasks",
        path: `/projects/${pathParts[1]}/tasks`,
      });
    }

    if (pathParts.length >= 4 && pathParts[2] === "tasks") {
      crumbs.push({
        label: "Task Details",
        path: `/projects/${pathParts[1]}/tasks/${pathParts[3]}`,
      });
    }

    if (pathParts.length >= 5 && pathParts[4] === "timeline") {
      crumbs.push({
        label: "Timeline",
        path: location.pathname,
      });
    }

    return crumbs;
  };

  const breadcrumbs = buildBreadcrumbs();

  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
      <AppBar position="static">
        <Toolbar>
          <Typography
            variant="h6"
            component={Link}
            to="/"
            sx={{ textDecoration: "none", color: "inherit", flexGrow: 1 }}
          >
            Mini Tasks
          </Typography>
          <Stack direction="row" spacing={1} alignItems="center">
            <Typography variant="body2" sx={{ color: "inherit" }}>
              {user?.fullName}
            </Typography>
            <Chip
              label={ROLES[user?.role] || user?.role}
              size="small"
              color={isAdmin ? "warning" : "default"}
              sx={{ color: "inherit", borderColor: "rgba(255,255,255,0.5)" }}
              variant="outlined"
            />
            <Button color="inherit" size="small" startIcon={<LogoutIcon />} onClick={logout}>
              Logout
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ mt: 2, mb: 4, flex: 1 }}>
        {breadcrumbs.length > 1 && (
          <Breadcrumbs sx={{ mb: 2 }}>
            {breadcrumbs.map((crumb, index) => {
              const isLast = index === breadcrumbs.length - 1;
              return isLast ? (
                <Typography key={crumb.path} color="text.primary">
                  {crumb.label}
                </Typography>
              ) : (
                <MuiLink
                  key={crumb.path}
                  component={Link}
                  to={crumb.path}
                  underline="hover"
                  color="inherit"
                >
                  {crumb.label}
                </MuiLink>
              );
            })}
          </Breadcrumbs>
        )}

        {children}
      </Container>
    </Box>
  );
};

export default Layout;
