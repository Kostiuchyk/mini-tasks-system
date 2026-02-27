export const TASK_STATUS = {
  New: "New",
  Active: "In Progress",
  Done: "Done",
};

export const TASK_STATUS_OPTIONS = Object.entries(TASK_STATUS).map(([value, label]) => ({
  value,
  label,
}));

export const TASK_PRIORITY = {
  Low: "Low",
  Medium: "Medium",
  High: "High",
};

export const TASK_PRIORITY_OPTIONS = Object.entries(TASK_PRIORITY).map(([value, label]) => ({
  value,
  label,
}));

export const STATUS_COLORS = {
  New: "default",
  Active: "primary",
  Done: "success",
};

export const PRIORITY_COLORS = {
  Low: "info",
  Medium: "warning",
  High: "error",
};

export const AUDIT_ACTION_LABELS = {
  created: "Task Created",
  status_changed: "Status Changed",
  priority_changed: "Priority Changed",
  assignee_changed: "Assignee Changed",
  comment_added: "Comment Added",
};

export const ROLES = {
  Admin: "Admin",
  Member: "Member",
};
