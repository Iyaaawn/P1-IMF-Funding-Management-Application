import { useEffect, useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { getErrorMessage } from "../api/client";
import { referenceApi, requestsApi } from "../api/endpoints";
import type { Country, LendingFacility } from "../types";

export default function NewRequestPage() {
  const navigate = useNavigate();
  const [countries, setCountries] = useState<Country[]>([]);
  const [facilities, setFacilities] = useState<LendingFacility[]>([]);
  const [countryId, setCountryId] = useState("");
  const [facilityId, setFacilityId] = useState("");
  const [title, setTitle] = useState("");
  const [purpose, setPurpose] = useState("");
  const [amount, setAmount] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);

  // Load both dropdowns in parallel, once
  useEffect(() => {
    Promise.all([referenceApi.countries(), referenceApi.facilities()])
      .then(([c, f]) => {
        setCountries(c);
        setFacilities(f);
      })
      .catch((err) => setError(getErrorMessage(err)));
  }, []);

  // Derived values: computed from state on each render, not stored separately
  const country = countries.find((c) => c.id === Number(countryId));
  const facility = facilities.find((f) => f.id === Number(facilityId));
  const limit = country ? country.quotaAmountSdrMillions * 2 : null; // 200% of quota, same rule as the API

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError("");
    setBusy(true);
    try {
      await requestsApi.create({
        countryId: Number(countryId),
        lendingFacilityId: Number(facilityId),
        title,
        purpose,
        requestAmountSdrMillions: Number(amount),
      });
      navigate("/");
    } catch (err) {
      setError(getErrorMessage(err)); // shows the API's message, e.g. the access-limit error
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card narrow">
      <h1>New funding request</h1>
      <form onSubmit={handleSubmit} className="form">
        <label htmlFor="country">Country</label>
        <select id="country" required value={countryId} onChange={(e) => setCountryId(e.target.value)}>
          <option value="">Choose a country</option>
          {countries.map((c) => (
            <option key={c.id} value={c.id}>{c.name} ({c.isoCode})</option>
          ))}
        </select>
        {country && (
          <p className="hint">
            Quota: {country.quotaAmountSdrMillions.toLocaleString()} m SDR · Max request: {limit!.toLocaleString()} m SDR
          </p>
        )}

        <label htmlFor="facility">Lending facility</label>
        <select id="facility" required value={facilityId} onChange={(e) => setFacilityId(e.target.value)}>
          <option value="">Choose a facility</option>
          {facilities.map((f) => (
            <option key={f.id} value={f.id}>{f.codeName}: {f.fullName}</option>
          ))}
        </select>
        {facility && <p className="hint">{facility.description} Up to {facility.maxTermMonths} months.</p>}

        <label htmlFor="title">Title</label>
        <input id="title" required maxLength={200} value={title} onChange={(e) => setTitle(e.target.value)} />

        <label htmlFor="purpose">Purpose</label>
        <textarea id="purpose" required rows={4} maxLength={2000} value={purpose} onChange={(e) => setPurpose(e.target.value)} />

        <label htmlFor="amount">Amount (millions of SDR)</label>
        <input id="amount" type="number" required min="0.01" step="0.01" value={amount} onChange={(e) => setAmount(e.target.value)} />

        {error && <p className="error" role="alert">{error}</p>}
        <div className="row">
          <button type="submit" disabled={busy}>{busy ? "Submitting..." : "Submit request"}</button>
          <button type="button" className="ghost" onClick={() => navigate("/")}>Cancel</button>
        </div>
      </form>
    </div>
  );
}
