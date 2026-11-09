import { Link } from "react-router"
import { ArrowLeft } from "lucide-react"
import { TopicForm } from "@/components/topic-form"
import { PageHeader } from "@/components/page-header"

export function CreateTopicPage() {
  return (
    <>
      <Link to="/topics" className="back-link">
        <ArrowLeft size={16} />
        All topics
      </Link>
      <PageHeader
        title="New topic"
        description="Create a standalone topic, ready to be reused wherever it belongs."
      />
      <TopicForm />
    </>
  )
}
