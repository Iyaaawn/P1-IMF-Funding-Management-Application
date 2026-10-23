import { api } from "./client";
import type {
  AuthResponse,
  Country,
  CreateFundingRequest,
  FundingRequest,
  LendingFacility,
  LoginRequest,
  RegisterRequest,
  Status,
} from "../types";

// Typed API calls: each function says exactly what it sends and what it gets back.

export const authApi = {
  login: (body: LoginRequest) => api.post<AuthResponse>("/auth/login", body).then((r) => r.data),
  register: (body: RegisterRequest) => api.post<AuthResponse>("/auth/register", body).then((r) => r.data),
};

export const referenceApi = {
  countries: () => api.get<Country[]>("/countries").then((r) => r.data),
  facilities: () => api.get<LendingFacility[]>("/lendingfacilities").then((r) => r.data),
};

export const requestsApi = {
  list: (status?: Status) =>
    api.get<FundingRequest[]>("/fundingrequests", { params: { status } }).then((r) => r.data),
  create: (body: CreateFundingRequest) =>
    api.post<FundingRequest>("/fundingrequests", body).then((r) => r.data),
  changeStatus: (id: number, status: Status) =>
    api.patch<FundingRequest>(`/fundingrequests/${id}/status`, { status }).then((r) => r.data),
  remove: (id: number) => api.delete(`/fundingrequests/${id}`),
};
