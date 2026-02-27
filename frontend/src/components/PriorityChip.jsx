import { Chip } from "@mui/material";
import { TASK_PRIORITY, PRIORITY_COLORS } from "../constants";

const PriorityChip = ({ priority, ...props }) => {
  return (
    <Chip
      label={TASK_PRIORITY[priority] || priority}
      color={PRIORITY_COLORS[priority] || "default"}
      size="small"
      variant="outlined"
      {...props}
    />
  );
};

export default PriorityChip;
