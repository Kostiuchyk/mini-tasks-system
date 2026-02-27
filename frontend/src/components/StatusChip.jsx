import { Chip } from "@mui/material";
import { TASK_STATUS, STATUS_COLORS } from "../constants";

const StatusChip = ({ status, ...props }) => {
  return (
    <Chip
      label={TASK_STATUS[status] || status}
      color={STATUS_COLORS[status] || "default"}
      size="small"
      {...props}
    />
  );
};

export default StatusChip;
