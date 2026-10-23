// TypeScript shapes that match the C# DTOs exactly.
// ASP.NET Core sends JSON in camelCase, so "CountryName" in C# arrives as "countryName" here.

export type Status =
  | "Submitted"
  | "StaffAssessment"
  | "BoardReview"
  | "Approved"
  | "Rejected"
  | "Disbursing"
  | "Completed";

export interface FundingRequest {
  id: number;
  countryId: number;
  countryName: string;
  lendingFacilityId: number;
  facilityCode: string;
  facilityName: string;
  title: string;
  purpose: string;
  requestAmountSdrMillions: number;
  status: Status;
  submittedDate: string; // dates arrive as ISO strings in JSON
  lastUpdated: string;
}

export interface CreateFundingRequest {
  countryId: number;
  lendingFacilityId: number;
  title: string;
  purpose: string;
  requestAmountSdrMillions: number;
}

export interface Country {
  id: number;
  name: string;
  isoCode: string;
  quotaAmountSdrMillions: number;
}

export interface LendingFacility {
  id: number;
  codeName: string;
  fullName: string;
  description: string;
  maxTermMonths: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  countryId?: number;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  userId: number;
  fullName: string;
  email: string;
  role: "Admin" | "Requester";
}

// Mirrors the AllowedTransitions dictionary in FundingRequestService,
// so the UI only offers moves the API will accept.
export const allowedTransitions: Record<Status, Status[]> = {
  Submitted: ["StaffAssessment", "Rejected"],
  StaffAssessment: ["BoardReview", "Rejected"],
  BoardReview: ["Approved", "Rejected"],
  Approved: ["Disbursing"],
  Disbursing: ["Completed"],
  Rejected: [],
  Completed: [],
};
