import { Box, CircularProgress, Typography } from "@mui/material";

export const LoadingState = () => (
  <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
    <CircularProgress />
  </Box>
);

export const ErrorState = ({ message = "Something went wrong" }) => (
  <Box sx={{ py: 4, textAlign: "center" }}>
    <Typography color="error">{message}</Typography>
  </Box>
);
