import { useState } from "react"
import { Link } from "react-router"
import { ArrowUpRight, BookOpen } from "lucide-react"
import { useGetBooks } from "@/api/generated/api"
import { RequestState } from "@/components/request-state"
import { PageHeader } from "@/components/page-header"
import { SearchField } from "@/components/search-field"
import { formatDate } from "@/utils/format-date"

export function BookListPage() {
  const { data: books = [], isLoading, isError } = useGetBooks()
  const [search, setSearch] = useState("")
  const filtered = books.filter((book) =>
    book.title.toLowerCase().includes(search.toLowerCase())
  )
  return (
    <>
      <PageHeader
        title="Your books"
        description="Bring reusable topics together into clear, structured books."
      />
      <div className="collection-toolbar">
        <SearchField
          value={search}
          onChange={setSearch}
          placeholder="Search books by title…"
        />
        <span>
          {filtered.length} {filtered.length === 1 ? "book" : "books"}
        </span>
      </div>
      {isLoading ? (
        <RequestState label="Opening your bookshelf" />
      ) : isError ? (
        <RequestState error label="We couldn’t load your books" />
      ) : filtered.length === 0 ? (
        <div className="empty-state">
          <BookOpen />
          <h2>{search ? "No matching books" : "Your bookshelf is empty"}</h2>
          <p>
            {search
              ? "Try another query or clear your search."
              : "Books will appear here when they are added to your library."}
          </p>
        </div>
      ) : (
        <div className="book-grid">
          {filtered.map((book, index) => (
            <Link to={`/books/${book.id}`} className="book-card" key={book.id}>
              <div className={`book-cover cover-${index % 3}`}>
                <span className="cover-kicker">AUTHORIT / BOOK</span>
                <BookOpen className="cover-icon" size={64} strokeWidth={1} />
                <span className="cover-number">
                  {String(book.id)}
                </span>
              </div>
              <div className="book-card-content">
                <div className="book-card-heading">
                  <h2>{book.title.trim()}</h2>
                  <ArrowUpRight size={19} />
                </div>
                <p>
                  {book.description}
                </p>
                <div className="card-footer">
                  <span>Created {formatDate(book.created)}</span>
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}
    </>
  )
}
