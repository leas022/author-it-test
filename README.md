# author-it-test

## API errors

Controller errors use Problem Details (`application/problem+json`), with `type`,
`title`, and `status`, plus `detail` where appropriate. Validation errors also
include an `errors` dictionary keyed by JSON field name (for example, `title`,
`tags`, `position`, or `level`), whose values are arrays of error messages.

- `400 Bad Request`: invalid input, including an invalid reference position or hierarchy.
- `404 Not Found`: the requested book, topic, or reference does not exist. This also applies when adding a reference to a nonexistent topic.
- `409 Conflict`: the submitted topic version is stale, so the update conflicts with the current resource state.
