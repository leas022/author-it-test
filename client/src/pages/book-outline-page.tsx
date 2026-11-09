import {
  deleteReference,
  getGetBookByIdQueryKey,
  useGetBookById,
} from "@/api/generated/api"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "react-toastify"
import { ApiError } from "@/api/api-fetch"
import { Link, useParams } from "react-router"
import { ArrowLeft, FileText, ListTree, Plus, Trash2 } from "lucide-react"
import { RequestState } from "@/components/request-state"
import { PageHeader } from "@/components/page-header"
import { formatDate } from "@/utils/format-date"

export function BookOutlinePage() {
  const { id } = useParams()
  const queryClient = useQueryClient()
  const removeReference = useMutation({
    mutationFn: ({
      bookId,
      referenceId,
      topicTitle,
    }: {
      bookId: number | string
      referenceId: number | string
      topicTitle: string
    }) =>
      toast.promise(deleteReference(bookId, referenceId), {
        pending: `Removing reference to “${topicTitle}”…`,
        success: `Successfully removed reference to “${topicTitle}”.`,
        error: {
          render: ({ data }) =>
            data instanceof ApiError
              ? data.message
              : "Could not remove the reference. Please try again.",
        },
      }),
    onSuccess: (updatedBook, { bookId }) => {
      queryClient.setQueryData(getGetBookByIdQueryKey(bookId), updatedBook)
    },
    // Reload the outline even after a failed request, and refresh usage reports.
    onSettled: () => queryClient.invalidateQueries(),
  })
  const {
    data: book,
    isLoading,
    isError,
  } = useGetBookById(id ?? "", { query: { enabled: Boolean(id) } })
  if (!id) return <RequestState error label="Book ID is missing" />
  if (isLoading) return <RequestState label="Opening book outline" />
  if (isError || !book)
    return <RequestState error label="We couldn’t open this book" />
  return (
    <>
      <Link className="back-link" to="/books">
        <ArrowLeft size={16} />
        All books
      </Link>
      <PageHeader
        title={book.title.trim()}
        description={book.description}
        action={
          <Link
            className="primary-link"
            to={`/books/${book.id}/references/new`}
          >
            <Plus size={16} />
            Add reference
          </Link>
        }
      />
      <div className="detail-meta">
        <span>
          {book.contents.length} references&nbsp;&nbsp;|&nbsp;&nbsp;Created{" "}
          {formatDate(book.created)}
        </span>
      </div>
      <section className="surface">
        {book.contents.length === 0 ? (
          <div className="empty-state">
            <ListTree />
            <p>This book has no contents yet.</p>
          </div>
        ) : (
          <ol className="outline-list">
            {book.contents.map((content) => (
              <li key={content.referenceId}>
                <div className="outline-row">
                  <Link
                    to={`/topics/${content.topicId}`}
                    className="outline-topic-link"
                  >
                    <span className="row-number">
                      {String(content.position).padStart(2, "0")}
                    </span>
                    <div
                      className="outline-title"
                      style={{
                        paddingInlineStart: `${Math.max(0, Number(content.level) - 1) * 24}px`,
                      }}
                    >
                      <FileText size={18} />
                      <span>{content.topicTitle}</span>
                    </div>
                    <span className="level-label">Level {content.level}</span>
                  </Link>
                  <button
                    type="button"
                    className="delete-reference-button"
                    aria-label={`Delete reference to ${content.topicTitle} at position ${content.position}`}
                    title="Delete reference"
                    disabled={removeReference.isPending}
                    onClick={() =>
                      removeReference.mutate({
                        bookId: book.id,
                        referenceId: content.referenceId,
                        topicTitle: content.topicTitle,
                      })
                    }
                  >
                    <Trash2 size={16} aria-hidden="true" />
                  </button>
                </div>
              </li>
            ))}
          </ol>
        )}
      </section>
    </>
  )
}
