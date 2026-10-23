import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { getErrorMessage } from "../api/client";
import { requestsApi } from "../api/endpoints";
import StatusBadge, { statusLabel } from "../components/StatusBadge";
import { allowedTransitions, type FundingRequest, type Status } from "../types";

const ALL_STATUSES: Status[] = [
  "Submitted", "StaffAssessment", "BoardReview", "Approved", "Rejected", "Disbursing", "Completed",
];

export default function RequestsPage() {
  const { isAdmin } = useAuth();
  const [requests, setRequests] = useState<FundingRequest[]>([]);
  const [filter, setFilter] = useState<Status | "">("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Re-fetch whenever the status filter changes (it's in the dependency array)
  useEffect(() => {
    setLoading(true);
    setError("");
    requestsApi
      .list(filter || undefined)
      .then(setRequests)
      .catch((err) => setError(getErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [filter]);

  // Replace one item in the list with the updated version from the API (no mutation)
  function replace(updated: FundingRequest) {
    setRequests((prev) => prev.map((r) => (r.id === updated.id ? updated : r)));
  }

  async function changeStatus(id: number, status: Status) {
    setError("");
    try {
      replace(await requestsApi.changeStatus(id, status));
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  async function remove(id: number) {
    setError("");
    try {
      await requestsApi.remove(id);
      setRequests((prev) => prev.filter((r) => r.id !== id));
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  return (
    <div className="card">
      <div className="page-head">
        <h1>Funding requests</h1>
        <Link to="/new" className="button">New request</Link>
      </div>

      <div className="filters">
        <label htmlFor="filter">Status</label>
        <select id="filter" value={filter} onChange={(e) => setFilter(e.target.value as Status | "")}>
          <option value="">All</option>
          {ALL_STATUSES.map((s) => (
            <option key={s} value={s}>{statusLabel(s)}</option>
          ))}
        </select>
      </div>

      {error && <p className="error" role="alert">{error}</p>}

      {loading ? (
        <p className="muted">Loading...</p>
      ) : requests.length === 0 ? (
        <p className="muted">No funding requests yet. <Link to="/new">Submit the first one.</Link></p>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Title</th>
                <th>Country</th>
                <th>Facility</th>
                <th className="num">Amount (SDR m)</th>
                <th>Status</th>
                <th>Submitted</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {requests.map((r) => (
                <tr key={r.id}>
                  <td>
                    <strong>{r.title}</strong>
                    <div className="muted small">{r.purpose}</div>
                  </td>
                  <td>{r.countryName}</td>
                  <td title={r.facilityName}>{r.facilityCode}</td>
                  <td className="num">{r.requestAmountSdrMillions.toLocaleString()}</td>
                  <td><StatusBadge status={r.status} /></td>
                  <td>{new Date(r.submittedDate).toLocaleDateString()}</td>
                  <td>
                    <div className="actions">
                      {/* Only admins can move requests through the workflow */}
                      {isAdmin &&
                        allowedTransitions[r.status].map((next) => (
                          <button
                            key={next}
                            className={next === "Rejected" ? "small danger" : "small"}
                            onClick={() => changeStatus(r.id, next)}
                          >
                            {statusLabel(next)}
                          </button>
                        ))}
                      {(r.status === "Submitted" || r.status === "Rejected") && (
                        <button className="small ghost" onClick={() => remove(r.id)}>Delete</button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
