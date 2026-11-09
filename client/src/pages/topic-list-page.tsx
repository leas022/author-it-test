import { useState } from "react"
import { Link } from "react-router"
import { ArrowUpRight, FileText, Plus } from "lucide-react"
import { useGetTopics } from "@/api/generated/api"
import { RequestState } from "@/components/request-state"
import { PageHeader } from "@/components/page-header"
import { SearchField } from "@/components/search-field"
import { formatDate } from "@/utils/format-date"

export function TopicListPage() {
  const { data: topics = [], isLoading, isError } = useGetTopics()
  const [search, setSearch] = useState("")
  const term = search.toLowerCase().trim()
  const filtered = topics.filter(
    (topic) =>
      topic.title.toLowerCase().includes(term) ||
      topic.tags.some((tag) => tag.toLowerCase().includes(term))
  )
  return (
    <>
      <PageHeader
        title="Your topics"
        description="Create the building blocks of your content. Write once, reuse across every book."
        action={
          <Link className="primary-link" to="/topics/new">
            <Plus size={17} />
            Create topic
          </Link>
        }
      />
      <div className="collection-toolbar">
        <SearchField
          value={search}
          onChange={setSearch}
          placeholder="Search topics by title or tag…"
        />
        <span>
          {filtered.length} {filtered.length === 1 ? "topic" : "topics"}
        </span>
      </div>
      {isLoading ? (
        <RequestState label="Gathering your topics" />
      ) : isError ? (
        <RequestState error label="We couldn’t load your topics" />
      ) : (
        <section className="surface">
          {filtered.length === 0 ? (
            <div className="empty-state">
              <FileText />
              <h2>
                {search ? "No matching topics" : "Start with a single idea."}
              </h2>
              <p>
                {search
                  ? "Try a different title or tag."
                  : "Create your first topic to begin building your library."}
              </p>
            </div>
          ) : (
            <ul className="topic-list">
              {filtered.map((topic) => (
                <li key={topic.id}>
                  <Link className="topic-row" to={`/topics/${topic.id}`}>
                    <span className="topic-icon">
                      <FileText size={20} />
                    </span>
                    <div className="topic-summary">
                      <h2>{topic.title}</h2>
                      <div className="tag-list">
                        {topic.tags.map((tag) => (
                          <span className="tag" key={tag}>
                            {tag}
                          </span>
                        ))}
                      </div>
                    </div>
                    <span className="updated-date">
                      Updated {formatDate(topic.updated)}
                    </span>
                    <ArrowUpRight size={18} />
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </section>
      )}
    </>
  )
}
