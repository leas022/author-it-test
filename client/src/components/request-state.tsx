import { AlertCircle, LoaderCircle } from "lucide-react"

export function RequestState({
  error = false,
  label,
}: {
  error?: boolean
  label: string
}) {
  return (
    <div
      className={`request-state ${error ? "request-error" : ""}`}
      role={error ? "alert" : "status"}
    >
      {error ? (
        <AlertCircle size={22} />
      ) : (
        <LoaderCircle size={22} className="motion-safe:animate-spin" />
      )}
      <div>
        <strong>{label}</strong>
        <p>
          {error
            ? "Please check your connection and try refreshing the page."
            : "Your library will be ready in a moment."}
        </p>
      </div>
    </div>
  )
}

