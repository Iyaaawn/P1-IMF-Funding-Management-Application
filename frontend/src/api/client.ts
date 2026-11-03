import axios from "axios";

const TOKEN_KEY = "imf_auth";

// One shared axios instance. Every call goes to the API's base URL.
export const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL ?? "https://localhost:7297"}/api`,
});

// Interceptor: before EVERY request, attach the JWT if we have one.
// This is where "the client sends the token in the Authorization header" actually happens.
api.interceptors.request.use((config) => {
  const saved = localStorage.getItem(TOKEN_KEY);
  if (saved) {
    const { token } = JSON.parse(saved) as { token: string };
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export { TOKEN_KEY };

// Turns any API error into one readable sentence for the UI.
export function getErrorMessage(err: unknown): string {
  if (axios.isAxiosError(err)) {
    if (!err.response) return "Can't reach the API. Is the backend running?";
    const data = err.response.data;
    // Our service errors: { message: "..." }
    if (data?.message) return data.message;
    // [ApiController] validation errors (Problem Details): { errors: { Title: ["..."] } }
    if (data?.errors) return Object.values(data.errors as Record<string, string[]>).flat().join(" ");
    if (err.response.status === 401) return "Please log in again.";
    if (err.response.status === 403) return "You don't have permission to do that.";
    return `Request failed (${err.response.status}).`;
  }
  return "Something went wrong.";
}
