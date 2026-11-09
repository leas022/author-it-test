# Author-it practical test
Luca Eastwood
## Running locally

Prerequisites:
- .NET 10 SDK.
- Node.js and npm (I used Node v26.13.0).
- The backend files and the `client` source directory, including its package lockfile.

From the repository root, install the frontend dependencies once:

```sh
npm install --prefix client
```

Then run these two commands in separate terminals, both from the repository root:

```sh
dotnet run --project AuthorIt.csproj --launch-profile http
```

```sh
npm run dev --prefix client
```

Open **http://localhost:8081** to access the frontend. Vite proxies /api/* requests to the backend API on port 8080. No CORS policy is configured because the browser sees same-origin requests.

The API port is configured in `Properties/launchSettings.json`. The frontend port and API proxy target are configured in `client/vite.config.ts`. Vite uses a strict port, so it will fail to start if port 8081 is occupied.

### Build and regenerate the API client

From the repository root:

```sh
dotnet build AuthorIt.csproj
npm run build --prefix client
```

The frontend build checks TypeScript and writes static assets to `client/dist`.

The OpenAPI document is available at **http://localhost:8080/openapi/v1.json**. With the API running, regenerate the TypeScript models, fetch functions, and TanStack Query hooks after changing the API contract:

```sh
npm run generate:api --prefix client
```

Generated files live in `client/src/api/generated`. The custom `api-fetch.ts` wrapper converts unsuccessful responses into `ApiError` objects so forms can display API validation and conflict errors.

## Design decisions and trade-offs

### Backend and domain model

ASP.NET Core on .NET 10 provides controllers, dependency injection, DTO validation, and OpenAPI without additional infrastructure. Controllers handle HTTP responses; services handle domain operations; DTOs and mappings separate the API contract from mutable stored models.

`LibraryStore` is a singleton in-memory store. This keeps the exercise easy to run, but data is temporary and the dictionaries/lists are not safe for concurrent mutation.

Books store a flat ordered list of references with a separate `referenceId` for every occurrence. A reference stores only the topic ID and level; the current title is resolved from the topic when returning an outline. This supports repeat references without copying topic content. Positions are computed from list order, starting at 1, so insertion and deletion naturally close gaps.

### Frontend

React and TypeScript match the preferred stack in the brief. Vite provides the development server and build, React Router handles pages, and TanStack Query manages fetched server state. Local React state holds editable form values and the saved baseline, allowing unsaved-change detection and preserving edits after a rejected save.

Orval generates the client from the C# OpenAPI contract to reduce duplicated request/response types. Successful mutations invalidate cached queries; query data has `staleTime: 0`, allowing refetches when views mount or regain focus. This keeps outlines and reuse information current after edits without maintaining a separate global state store. It is not live push synchronization between users.

Tags are entered as comma-separated values, trimmed and converted to an array. Filtering happens in the browser, which is suitable for this small dataset. Styling uses custom CSS with Tailwind, Lucide icons, and react-toastify notifications.

## API contract and status codes

All endpoints use the `/api` prefix:

| Method | Path | Success response |
| --- | --- | --- |
| GET | `/api/topics` | `200`: all topics |
| GET | `/api/topics/{id}` | `200`: one topic |
| POST | `/api/topics` | `201`: created topic |
| PUT | `/api/topics/{id}` | `200`: updated topic |
| GET | `/api/books` | `200`: books without contents |
| GET | `/api/books/{id}` | `200`: book with ordered contents |
| POST | `/api/books` | `201`: created book with empty contents |
| POST | `/api/books/{id}/references` | `201`: updated book with contents |
| DELETE | `/api/books/{id}/references/{referenceId}` | `200`: updated book with contents |
| GET | `/api/topics/{id}/usage` | `200`: containing books, every reference, and totals |

Creation uses `201 Created` and a `Location` header. For reference creation, that location is the containing book because no standalone reference GET endpoint exists. Updates and reference deletion return `200` with the resulting resource instead of `204`, giving the client the current state immediately.

Controller errors use Problem Details (`application/problem+json`), with `type`, `title`, and `status`, plus `detail` where appropriate. Validation errors also include an `errors` dictionary whose values are message arrays.

- **400 Bad Request:** invalid input, including invalid position or hierarchy.
- **404 Not Found:** a requested book, topic, or reference is missing, including a nonexistent topic supplied when adding a reference. This treats the referenced topic as a missing resource rather than a field-format error.
- **409 Conflict:** the submitted topic version is stale. This describes a conflict with current resource state; `412 Precondition Failed` would be a better fit if the API used an `If-Match` header. A sequential stale update is rejected without applying its edits.

## Removing references and deleting topics

**Reference deletion removes only the selected entry.** Entries indented beneath it are kept and promoted by one level. This prevents accidental loss of content when a reference is removed.

## Service unit tests

Run the xUnit service tests from the repository root:

```sh
dotnet test tests/AuthorIt.Tests/AuthorIt.Tests.csproj
```

Each test uses data from a fresh `LibraryStore`.  The tests each focus on one specific aspect of the service logic. Ideally integration tests would be added to cover status codes and input validation.