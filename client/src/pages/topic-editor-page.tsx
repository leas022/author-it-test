import { Link, useParams } from "react-router"
import { ArrowLeft, GitBranch } from "lucide-react"
import { useGetTopicById, useGetTopicUsage } from "@/api/generated/api"
import { RequestState } from "@/components/request-state"
import { TopicForm } from "@/components/topic-form"
import { PageHeader } from "@/components/page-header"

export function TopicEditorPage() {
  const { id } = useParams()
  const {
    data: topic,
    isLoading,
    isError,
  } = useGetTopicById(id ?? "", { query: { enabled: Boolean(id) } })
  const usage = useGetTopicUsage(id ?? "", { query: { enabled: Boolean(id) } })
  if (isLoading) return <RequestState label="Opening your topic" />
  if (!id || isError || !topic)
    return <RequestState error label="We couldn’t open this topic" />
  return (
    <>
      <Link className="back-link" to="/topics">
        <ArrowLeft size={16} />
        All topics
      </Link>
      <PageHeader
        title="Edit topic"
        description="A single source for your content. Changes carry through to every book that uses it."
      />
      <div className="reuse-banner">
        <GitBranch size={20} />
        <div>
          <strong>
            {usage.isError
              ? "Usage information unavailable"
              : !usage.data
                ? "Checking where this topic is used…"
                : Number(usage.data.bookCount) === 0
                  ? "Ready to be reused"
                  : `Used in ${usage.data.bookCount} ${Number(usage.data.bookCount) === 1 ? "book" : "books"}`}
          </strong>
          <p>
            {usage.isError
              ? "You can still edit, but we couldn’t check which books will be affected."
              : usage.data && Number(usage.data.bookCount) > 1
                ? "Saving changes updates this topic in all of these books."
                : "Books reference this topic without making a separate copy."}
          </p>
          <div className="reuse-links">
            {usage.data?.books.map((book) => (
              <Link key={book.bookId} to={`/books/${book.bookId}`}>
                {book.bookTitle} ↗
              </Link>
            ))}
          </div>
        </div>
      </div>
      <TopicForm key={topic.id} topic={topic} />
    </>
  )
}
