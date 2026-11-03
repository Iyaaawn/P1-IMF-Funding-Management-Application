import type { Status } from "../types";

// "StaffAssessment" -> "Staff Assessment"
export const statusLabel = (s: Status) => s.replace(/([a-z])([A-Z])/g, "$1 $2");

export default function StatusBadge({ status }: { status: Status }) {
  return <span className={`badge badge-${status}`}>{statusLabel(status)}</span>;
}
