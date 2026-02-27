import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ThemeProvider, createTheme, CssBaseline } from "@mui/material";
import { AuthProvider, useAuth } from "./context/AuthContext";
import Layout from "./components/Layout";
import LoginPage from "./pages/Login/LoginPage";
import ProjectList from "./pages/Projects/ProjectList";
import TaskList from "./pages/Tasks/TaskList";
import TaskDetails from "./pages/TaskDetails/TaskDetails";
import TaskTimeline from "./pages/TaskTimeline/TaskTimeline";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: false,
    },
  },
});

const theme = createTheme({
  palette: {
    mode: "light",
  },
});

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return children;
};

const App = () => {
  return (
    <AuthProvider>
      <QueryClientProvider client={queryClient}>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <BrowserRouter>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route
                path="/*"
                element={
                  <ProtectedRoute>
                    <Layout>
                      <Routes>
                        <Route path="/" element={<ProjectList />} />
                        <Route path="/projects/:projectId/tasks" element={<TaskList />} />
                        <Route path="/projects/:projectId/tasks/:taskId" element={<TaskDetails />} />
                        <Route path="/projects/:projectId/tasks/:taskId/timeline" element={<TaskTimeline />} />
                      </Routes>
                    </Layout>
                  </ProtectedRoute>
                }
              />
            </Routes>
          </BrowserRouter>
        </ThemeProvider>
      </QueryClientProvider>
    </AuthProvider>
  );
};

export default App;
