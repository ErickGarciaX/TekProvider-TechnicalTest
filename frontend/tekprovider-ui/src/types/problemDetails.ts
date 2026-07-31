export interface ProblemDetails {
  title: string;
  status: number;
  errorCode?: string;
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  readonly status: number;
  readonly errorCode?: string;
  readonly fieldErrors?: Record<string, string[]>;

  constructor(problemDetails: ProblemDetails) {
    super(problemDetails.title);
    this.status = problemDetails.status;
    this.errorCode = problemDetails.errorCode;
    this.fieldErrors = problemDetails.errors;
  }
}
