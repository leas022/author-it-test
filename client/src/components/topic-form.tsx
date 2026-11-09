import { useState, type FormEvent } from "react"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { Link, useNavigate } from "react-router"
import { Check, Save } from "lucide-react"
import { toast } from "react-toastify"
import { createTopic, updateTopic, getTopicById } from "@/api/generated/api"
import { ApiError } from "@/api/api-fetch"
import type { TopicResponse } from "@/api/generated/models"

export function TopicForm({ topic }: { topic?: TopicResponse }) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [title, setTitle] = useState(topic?.title ?? "")
  const [body, setBody] = useState(topic?.body ?? "")
  const [tags, setTags] = useState(topic?.tags.join(", ") ?? "")
  const [version, setVersion] = useState(topic?.version)
  const [baseline, setBaseline] = useState({ title, body, tags })
  const [fields, setFields] = useState<Record<string, string[]>>({})
  const [message, setMessage] = useState("")
  const [conflict, setConflict] = useState(false)
  const [reloading, setReloading] = useState(false)
  const [saved, setSaved] = useState(false)
  const dirty =
    title !== baseline.title || body !== baseline.body || tags !== baseline.tags
  const mutation = useMutation({
    mutationFn: () => {
      const input = {
        title,
        body,
        tags: tags
          .split(",")
          .map((tag) => tag.trim())
          .filter(Boolean),
      }
      return topic
        ? updateTopic(topic.id, { ...input, version: version! })
        : createTopic(input)
    },
    onSuccess: (result) => {
      toast.success(topic ? "Topic updated successfully." : "Topic created successfully.")
      setVersion(result.version)
      setBaseline({ title, body, tags })
      setSaved(true)
      setMessage("")
      setFields({})
      setConflict(false)
      void queryClient.invalidateQueries()
      if (!topic) navigate(`/topics/${result.id}`, { replace: true })
    },
    onError: (error) => {
      setSaved(false)
      setFields(error instanceof ApiError ? (error.problem.errors ?? {}) : {})
      setConflict(error instanceof ApiError && error.status === 409)
      setMessage(
        error instanceof ApiError
          ? error.message
          : "We couldn’t reach the library. Please try again."
      )
    },
  })
  async function reload() {
    if (
      !topic ||
      !window.confirm(
        "Replace your unsaved changes with the latest saved topic?"
      )
    )
      return
    setReloading(true)
    try {
      const latest = await getTopicById(topic.id)
      const next = {
        title: latest.title,
        body: latest.body,
        tags: latest.tags.join(", "),
      }
      setTitle(next.title)
      setBody(next.body)
      setTags(next.tags)
      setBaseline(next)
      setVersion(latest.version)
      setMessage("")
      setConflict(false)
      setFields({})
      setSaved(false)
      void queryClient.invalidateQueries()
    } catch {
      setMessage("Could not reload this topic. Your edits have been kept.")
    } finally {
      setReloading(false)
    }
  }
  function submit(event: FormEvent) {
    event.preventDefault()
    setSaved(false)
    mutation.mutate()
  }
  return (
    <form onSubmit={submit} className="surface editor-surface">
      <div className="surface-heading">
        <h2>Topic content</h2>
        <span role="status">
          {dirty
            ? "Unsaved changes"
            : saved
              ? "All changes saved"
              : topic
                ? `Version ${version}`
                : "New draft"}
        </span>
      </div>
      {message && (
        <div className="form-alert" role="alert">
          <p>{message}</p>
          {conflict && (
            <button
              type="button"
              className="text-button"
              onClick={() => void reload()}
              disabled={reloading}
            >
              {reloading ? "Reloading…" : "Reload latest version"}
            </button>
          )}
        </div>
      )}
      <fieldset
        className="editor-fields"
        disabled={mutation.isPending || reloading}
      >
        <label htmlFor="topic-title">
          Title
          <span className="field-hint">
            Give this topic a clear, descriptive name.
          </span>
        </label>
        <input
          id="topic-title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          placeholder="What is this topic about?"
          aria-invalid={Boolean(fields.title)}
          aria-describedby={fields.title ? "title-error" : undefined}
        />
        {fields.title && (
          <p id="title-error" className="field-error">
            {fields.title.join(" ")}
          </p>
        )}
        <label htmlFor="topic-body">
          Body
          <span className="field-hint">
            Write the content that will be shared across your books.
          </span>
        </label>
        <textarea
          id="topic-body"
          value={body}
          onChange={(event) => setBody(event.target.value)}
          rows={12}
          placeholder="Start writing your topic here…"
          aria-invalid={Boolean(fields.body)}
          aria-describedby={fields.body ? "body-error" : undefined}
        />
        {fields.body && (
          <p id="body-error" className="field-error">
            {fields.body.join(" ")}
          </p>
        )}
        <label htmlFor="topic-tags">
          Tags
          <span className="field-hint">
            Separate tags with commas to make this topic easier to find.
          </span>
        </label>
        <input
          id="topic-tags"
          value={tags}
          onChange={(event) => setTags(event.target.value)}
          placeholder="e.g. getting-started, authoring"
          aria-invalid={Boolean(fields.tags)}
          aria-describedby={fields.tags ? "tags-error" : undefined}
        />
        {fields.tags && (
          <p id="tags-error" className="field-error">
            {fields.tags.join(" ")}
          </p>
        )}
      </fieldset>
      <div className="editor-footer">
        <span>
          {body.length.toLocaleString()} characters
          {saved && !dirty && <Check size={16} />}
        </span>
        <div>
          <Link className="action-link" to="/topics">
            Back to topics
          </Link>
          <button
            className="primary-link"
            type="submit"
            disabled={
              mutation.isPending || reloading || Boolean(topic && !dirty)
            }
          >
            <Save size={16} />
            {mutation.isPending
              ? "Saving…"
              : topic
                ? "Save changes"
                : "Create topic"}
          </button>
        </div>
      </div>
    </form>
  )
}
