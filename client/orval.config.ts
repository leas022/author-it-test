import { defineConfig } from "orval"

export default defineConfig({
    authorIt: {
        input: {
            target: "http://localhost:8080/openapi/v1.json",
        },
        output: {
            target: "./src/api/generated/api.ts",
            schemas: "./src/api/generated/models",
            client: "react-query",
            httpClient: "fetch",
            baseUrl: "",
            override: {
                fetch: {
                    includeHttpResponseReturnType: false,
                },
                mutator: {
                    path: "./src/api/api-fetch.ts",
                    name: "apiFetch",
                },
            },
        },
    },
})