import { useState } from "react";
import { Navigate } from "react-router-dom";
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Tabs,
  Tab,
  Alert,
} from "@mui/material";
import { useAuth } from "../../context/AuthContext";

const LoginPage = () => {
  const { isAuthenticated, login, register } = useAuth();
  const [tab, setTab] = useState(0);
  const [error, setError] = useState("");

  const [loginForm, setLoginForm] = useState({ email: "", password: "" });
  const [registerForm, setRegisterForm] = useState({ fullName: "", email: "", password: "" });

  if (isAuthenticated) return <Navigate to="/" replace />;

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await login(loginForm.email, loginForm.password);
    } catch (err) {
      setError(err.response?.data?.detail || err.message);
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setError("");
    if (!registerForm.fullName.trim() || !registerForm.email.trim() || !registerForm.password.trim()) {
      setError("All fields are required");
      return;
    }
    try {
      await register(registerForm.fullName, registerForm.email, registerForm.password);
    } catch (err) {
      setError(err.response?.data?.detail || err.message);
    }
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        bgcolor: "grey.100",
      }}
    >
      <Paper sx={{ p: 4, maxWidth: 400, width: "100%" }}>
        <Typography variant="h5" align="center" gutterBottom>
          Mini Tasks
        </Typography>

        <Tabs value={tab} onChange={(_, v) => { setTab(v); setError(""); }} centered sx={{ mb: 3 }}>
          <Tab label="Sign In" />
          <Tab label="Register" />
        </Tabs>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {tab === 0 ? (
          <Box component="form" onSubmit={handleLogin}>
            <TextField
              label="Email"
              type="email"
              fullWidth
              margin="normal"
              value={loginForm.email}
              onChange={(e) => setLoginForm({ ...loginForm, email: e.target.value })}
              required
            />
            <TextField
              label="Password"
              type="password"
              fullWidth
              margin="normal"
              value={loginForm.password}
              onChange={(e) => setLoginForm({ ...loginForm, password: e.target.value })}
              required
            />
            <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
              Sign In
            </Button>
          </Box>
        ) : (
          <Box component="form" onSubmit={handleRegister}>
            <TextField
              label="Full Name"
              fullWidth
              margin="normal"
              value={registerForm.fullName}
              onChange={(e) => setRegisterForm({ ...registerForm, fullName: e.target.value })}
              required
            />
            <TextField
              label="Email"
              type="email"
              fullWidth
              margin="normal"
              value={registerForm.email}
              onChange={(e) => setRegisterForm({ ...registerForm, email: e.target.value })}
              required
            />
            <TextField
              label="Password"
              type="password"
              fullWidth
              margin="normal"
              value={registerForm.password}
              onChange={(e) => setRegisterForm({ ...registerForm, password: e.target.value })}
              required
            />
            <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
              Register
            </Button>
          </Box>
        )}
      </Paper>
    </Box>
  );
};

export default LoginPage;
