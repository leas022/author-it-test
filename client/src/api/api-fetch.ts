import type { ValidationProblemDetails } from "./generated/models"

export class ApiError extends Error {
    public readonly status: number
    public readonly problem: ValidationProblemDetails

  constructor(
    status: number,
    problem: ValidationProblemDetails
  ) {
    super(problem.detail ?? problem.title ?? `Request failed (${status})`)
    this.name = "ApiError"
    this.status = status
    this.problem = problem
  }
}

export type ErrorType<T> = ApiError & { problem: T }

export async function apiFetch<T>(
  url: string,
  options?: RequestInit,
): Promise<T> {
  const response = await fetch(url, options)

  if (!response.ok) {
    const problem: ValidationProblemDetails = await response.json().catch(() => ({
      title: response.statusText || "Request failed",
    }))

    throw new ApiError(response.status, problem)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}