# API Conventions

## Status

The API foundation implements shared error handling and structured request
logging. Product endpoints, authentication, and API versioning remain planned.

## Error Responses

API failures use the ASP.NET Core Problem Details media type:

```text
application/problem+json
```

Every shared error response contains a safe title, HTTP status, `code`, and
`traceId`. Clients can use `code` for predictable handling and provide the
`traceId` when reporting a problem. Internal exception messages and stack
traces are never returned to clients.

| Condition | HTTP status | `code` |
|---|---:|---|
| Invalid request data | 400 | `validation_failed` |
| Requested resource does not exist | 404 | `resource_not_found` |
| Request conflicts with current state | 409 | `resource_conflict` |
| Unexpected failure | 500 | `unexpected_error` |

Validation responses also include an `errors` object keyed by request field.

Future application use cases must express expected outcomes with the matching
application exception. They must not throw generic .NET exceptions to represent
ordinary validation, missing-resource, or conflict behavior.

## Logging

Every request records these structured fields at information level:

- `RequestMethod`
- `RequestPath`
- `StatusCode`
- `ElapsedMilliseconds`
- `TraceId`

Failures also record the safe error code, exception type, stack-trace location,
and trace ID at error level. The request logger must not record request or
response bodies, headers, query strings, passwords, tokens, or connection
strings.

Console logs use JSON formatting so local and future centralized logging tools
can query the named fields.

## Validation Boundary

HTTP request-shape validation belongs at the API boundary. Business rules and
state transitions belong in Application and Domain. Product-specific request
validators will be introduced with their corresponding endpoints; no placeholder
product endpoint exists solely to demonstrate this infrastructure.
