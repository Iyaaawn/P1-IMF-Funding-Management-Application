import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function NavBar() {
  const { user, isAdmin, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <header className="navbar">
      <Link to="/" className="brand">IMF Funding Management</Link>
      {user && (
        <nav>
          <Link to="/">Requests</Link>
          <Link to="/new">New request</Link>
          <span className="who">
            {user.fullName} <span className={`role ${isAdmin ? "admin" : ""}`}>{user.role}</span>
          </span>
          <button
            className="link-button"
            onClick={() => {
              logout();
              navigate("/login");
            }}
          >
            Log out
          </button>
        </nav>
      )}
    </header>
  );
}
