import { useState, type FormEvent } from "react"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { Link, useNavigate, useParams } from "react-router"
import { ArrowLeft, FileText, Plus } from "lucide-react"
import {
  createReference,
  getGetBookByIdQueryKey,
  useGetBookById,
  useGetTopics,
} from "@/api/generated/api"
import type {
  BookResponseWithContents,
  TopicResponse,
} from "@/api/generated/models"
import { ApiError } from "@/api/api-fetch"
import { PageHeader } from "@/components/page-header"
import { RequestState } from "@/components/request-state"

export function CreateReferencePage() {
  const { id } = useParams()
  const bookQuery = useGetBookById(id ?? "", {
    query: { enabled: Boolean(id) },
  })
  const topicsQuery = useGetTopics()

  if (!id) return <RequestState error label="Book ID is missing" />
  if (bookQuery.isLoading || topicsQuery.isLoading)
    return <RequestState label="Loading book and topics" />
  if (bookQuery.isError || !bookQuery.data)
    return <RequestState error label="We couldn’t load this book" />
  if (topicsQuery.isError || !topicsQuery.data)
    return <RequestState error label="We couldn’t load topics" />

  return (
    <ReferenceForm key={id} book={bookQuery.data} topics={topicsQuery.data} />
  )
}

function ReferenceForm({
  book,
  topics,
}: {
  book: BookResponseWithContents
  topics: TopicResponse[]
}) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [topicId, setTopicId] = useState("")
  const [level, setLevel] = useState("1")
  const [position, setPosition] = useState(String(book.contents.length + 1))
  const mutation = useMutation({
    mutationFn: () =>
      createReference(book.id, {
        topicId,
        level,
        position: position === "" ? null : position,
      }),
    onSuccess: (result) => {
      queryClient.setQueryData(getGetBookByIdQueryKey(book.id), result)
      // Refresh outlines and usage reports after the new reference is added.
      void queryClient.invalidateQueries()
      navigate(`/books/${book.id}`)
    },
  })
  const error = mutation.error
  const errors = error instanceof ApiError ? (error.problem.errors ?? {}) : {}
  const fieldError = (name: string) =>
    Object.entries(errors)
      .filter(
        ([key]) => key.split(".").pop()?.toLowerCase() === name.toLowerCase()
      )
      .flatMap(([, messages]) => messages)
      .join(" ")

  function submit(event: FormEvent) {
    event.preventDefault()
    mutation.mutate()
  }

  return (
    <>
      <Link className="back-link" to={`/books/${book.id}`}>
        <ArrowLeft size={16} />
        Book outline
      </Link>
      <PageHeader title="Add reference" description={book.title} />
      <div className="reference-layout">
        <section className="surface">
          <div className="surface-heading">
            <h2>Book contents</h2>
          </div>
          {book.contents.length === 0 ? (
            <div className="empty-state">
              <p>This book has no contents yet.</p>
            </div>
          ) : (
            <ol className="outline-list">
              {book.contents.map((content) => (
                <li key={content.referenceId}>
                  <div className="outline-row">
                    <span className="row-number">{content.position}</span>
                    <div
                      className="outline-title"
                      style={{
                        paddingInlineStart: `${Math.max(0, Number(content.level) - 1) * 24}px`,
                      }}
                    >
                      <FileText size={18} />
                      <span>{content.topicTitle}</span>
                    </div>
                    <span className="reference-level">
                      Level {content.level}
                    </span>
                  </div>
                </li>
              ))}
            </ol>
          )}
        </section>
        <form className="surface editor-surface" onSubmit={submit}>
          <div className="surface-heading">
            <h2>Reference</h2>
          </div>
          {error && (
            <div className="form-alert" role="alert">
              <p>
                {error instanceof ApiError
                  ? error.message
                  : "We couldn’t reach the library. Please try again."}
              </p>
              {Object.entries(errors)
                .filter(
                  ([key]) =>
                    !["topicid", "level", "position"].includes(
                      key.split(".").pop()?.toLowerCase() ?? ""
                    )
                )
                .map(([key, messages]) => (
                  <p key={key}>{messages.join(" ")}</p>
                ))}
            </div>
          )}
          <fieldset className="editor-fields" disabled={mutation.isPending}>
            <label htmlFor="reference-topic">Topic</label>
            <select
              id="reference-topic"
              value={topicId}
              onChange={(event) => setTopicId(event.target.value)}
              aria-invalid={Boolean(fieldError("topicId"))}
              aria-describedby={
                fieldError("topicId") ? "reference-topic-error" : undefined
              }
              required
            >
              <option value="" disabled>Select a topic</option>
              {topics.map((topic) => (
                <option key={topic.id} value={String(topic.id)}>
                  {topic.title}
                </option>
              ))}
            </select>
            {fieldError("topicId") && (
              <p id="reference-topic-error" className="field-error">
                {fieldError("topicId")}
              </p>
            )}
            {topics.length === 0 && <p>No topics available.</p>}
            <label htmlFor="reference-level">Level</label>
            <input
              id="reference-level"
              inputMode="numeric"
              value={level}
              required
              onChange={(event) => setLevel(event.target.value)}
              aria-invalid={Boolean(fieldError("level"))}
              aria-describedby={
                fieldError("level") ? "reference-level-error" : undefined
              }
            />
            {fieldError("level") && (
              <p id="reference-level-error" className="field-error">
                {fieldError("level")}
              </p>
            )}
            <label htmlFor="reference-position">Position</label>
            <input
              id="reference-position"
              inputMode="numeric"
              value={position}
              placeholder="Append to end"
              onChange={(event) => setPosition(event.target.value)}
              aria-invalid={Boolean(fieldError("position"))}
              aria-describedby={
                fieldError("position") ? "reference-position-error" : undefined
              }
            />
            {fieldError("position") && (
              <p id="reference-position-error" className="field-error">
                {fieldError("position")}
              </p>
            )}
          </fieldset>
          <div className="editor-footer">
            <div>
              <Link className="action-link" to={`/books/${book.id}`}>
                Cancel
              </Link>
              <button
                type="submit"
                className="primary-link"
                disabled={mutation.isPending}
              >
                <Plus size={16} />
                {mutation.isPending ? "Adding…" : "Add reference"}
              </button>
            </div>
          </div>
        </form>
      </div>
    </>
  )
}
