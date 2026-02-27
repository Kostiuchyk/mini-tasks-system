/**
 * Converts an array of row arrays to a CSV string and triggers a browser download.
 * @param {string[]} headers - Column header labels.
 * @param {(string|number)[]} rows[] - Each element is an array of cell values.
 * @param {string} filename - Downloaded file name (without extension).
 */
export const exportToCsv = (headers, rows, filename = "export") => {
  const escape = (val) => {
    const str = val == null ? "" : String(val);
    // Wrap in quotes if value contains commas, quotes, or newlines
    if (str.includes(",") || str.includes('"') || str.includes("\n")) {
      return `"${str.replace(/"/g, '""')}"`;
    }
    return str;
  };

  const lines = [
    headers.map(escape).join(","),
    ...rows.map((row) => row.map(escape).join(",")),
  ];

  const blob = new Blob([lines.join("\r\n")], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = `${filename}.csv`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
};
