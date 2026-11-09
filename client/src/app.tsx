import { Route, Routes, Navigate, NavLink, Outlet, Link } from "react-router"
import { BookOpen, FileText, Library } from "lucide-react"
import { TopicListPage } from "./pages/topic-list-page"
import { CreateTopicPage } from "./pages/create-topic-page"
import { TopicEditorPage } from "./pages/topic-editor-page"
import { BookListPage } from "./pages/book-list-page"
import { BookOutlinePage } from "./pages/book-outline-page"
import { CreateReferencePage } from "./pages/create-reference-page"

function Layout() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <Link to="/topics" className="brand">
          <span className="brand-mark">
            <Library size={22} />
          </span>
          Author-it
        </Link>
        <nav aria-label="Main navigation" className="main-nav">
          <NavLink to="/topics">
            <FileText size={18} />
            Topics
          </NavLink>
          <NavLink to="/books">
            <BookOpen size={18} />
            Books
          </NavLink>
        </nav>
      </aside>
      <div className="workspace">
        <main id="main-content" className="page-container" tabIndex={-1}>
          <Outlet />
        </main>
      </div>
    </div>
  )
}

export function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Navigate to="/topics" replace />} />
        <Route path="topics" element={<TopicListPage />} />
        <Route path="topics/new" element={<CreateTopicPage />} />
        <Route path="topics/:id" element={<TopicEditorPage />} />
        <Route path="books" element={<BookListPage />} />
        <Route path="books/:id" element={<BookOutlinePage />} />
        <Route
          path="books/:id/references/new"
          element={<CreateReferencePage />}
        />
        <Route
          path="*"
          element={
            <div className="empty-state">
              <span className="eyebrow">404 / NOT FOUND</span>
              <p>Head back to your topics to keep exploring.</p>
              <Link className="action-link" to="/topics">
                Back to topics
              </Link>
            </div>
          }
        />
      </Route>
    </Routes>
  )
}
export default App
