import { useEffect, useState, type FormEvent } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { getErrorMessage } from "../api/client";
import { referenceApi } from "../api/endpoints";
import type { Country } from "../types";

export default function RegisterPage() {
  const { user, register } = useAuth();
  const navigate = useNavigate();
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [countryId, setCountryId] = useState("");
  const [countries, setCountries] = useState<Country[]>([]);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  // Load the country dropdown once, when the page first renders
  useEffect(() => {
    referenceApi.countries().then(setCountries).catch(() => setCountries([]));
  }, []);

  if (user) return <Navigate to="/" replace />;

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError("");
    setBusy(true);
    try {
      await register({
        fullName,
        email,
        password,
        countryId: countryId ? Number(countryId) : undefined,
      });
      navigate("/");
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card narrow">
      <h1>Create an account</h1>
      <form onSubmit={handleSubmit} className="form">
        <label htmlFor="fullName">Full name</label>
        <input id="fullName" required value={fullName} onChange={(e) => setFullName(e.target.value)} />

        <label htmlFor="email">Email</label>
        <input id="email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />

        <label htmlFor="password">Password (at least 8 characters)</label>
        <input id="password" type="password" required minLength={8} value={password} onChange={(e) => setPassword(e.target.value)} />

        <label htmlFor="country">Country you represent (optional)</label>
        <select id="country" value={countryId} onChange={(e) => setCountryId(e.target.value)}>
          <option value="">None</option>
          {countries.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>

        {error && <p className="error" role="alert">{error}</p>}
        <button type="submit" disabled={busy}>{busy ? "Creating..." : "Create account"}</button>
      </form>
      <p className="muted">Already registered? <Link to="/login">Log in</Link></p>
    </div>
  );
}
