import { createContext, useContext, useState, type ReactNode } from "react";
import { TOKEN_KEY } from "../api/client";
import { authApi } from "../api/endpoints";
import type { AuthResponse, LoginRequest, RegisterRequest } from "../types";

interface AuthContextValue {
  user: AuthResponse | null;
  isAdmin: boolean;
  login: (body: LoginRequest) => Promise<void>;
  register: (body: RegisterRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

// Reads a saved login from localStorage, ignoring it if the token has expired.
function loadSavedUser(): AuthResponse | null {
  try {
    const saved = localStorage.getItem(TOKEN_KEY);
    if (!saved) return null;
    const user = JSON.parse(saved) as AuthResponse;
    if (new Date(user.expiresAt) <= new Date()) {
      localStorage.removeItem(TOKEN_KEY);
      return null;
    }
    return user;
  } catch {
    return null;
  }
}

// Context = app-wide state without prop drilling. Any component can call useAuth().
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthResponse | null>(loadSavedUser);

  function save(result: AuthResponse) {
    localStorage.setItem(TOKEN_KEY, JSON.stringify(result));
    setUser(result);
  }

  const value: AuthContextValue = {
    user,
    isAdmin: user?.role === "Admin",
    login: async (body) => save(await authApi.login(body)),
    register: async (body) => save(await authApi.register(body)),
    logout: () => {
      localStorage.removeItem(TOKEN_KEY);
      setUser(null);
    },
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside <AuthProvider>");
  return ctx;
}
