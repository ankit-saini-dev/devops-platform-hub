# API Conventions

## Status

The API foundation implements shared error handling, structured request logging,
and health endpoints. Product endpoints, authentication, and API versioning
remain planned.

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

## Health Endpoints

Health endpoints are unauthenticated operational endpoints. They return plain
text health status and do not return connection strings, credentials, exception
messages, or stack traces.

| Endpoint | Meaning | Healthy response | Unhealthy response |
|---|---|---:|---:|
| `GET /health/live` | The API process can accept requests. It does not check PostgreSQL. | `200 Healthy` | Not applicable while the process is running. |
| `GET /health/ready` | The API can open a PostgreSQL connection and execute `SELECT 1`. | `200 Healthy` | `503 Unhealthy` when PostgreSQL configuration is missing or the database cannot be used. |

Liveness is suitable for determining whether an API process should be restarted.
Readiness is suitable for determining whether the API should receive traffic
that depends on PostgreSQL. The endpoints do not replace future monitoring,
metrics, alerting, or deployment-health policies.

## Logging

Every non-health request records these structured fields at information level:

- `RequestMethod`
- `RequestPath`
- `StatusCode`
- `ElapsedMilliseconds`
- `TraceId`

The request logger excludes `/health/*` probes because infrastructure polling
would otherwise create routine log noise.

Unhandled API failures record the error code, trace ID, exception type,
redacted exception message, stack trace, and inner-exception details at error
level. The PostgreSQL readiness check records equivalent redacted diagnostic
details when it converts a database failure to `503 Unhealthy`. Client
responses remain generic in both cases.

The logging sanitizer removes carriage-return and line-feed characters from
request values and redacts common password, token, authorization, and
PostgreSQL URI credential patterns from exception details. It must not log
request or response bodies, headers, query strings, passwords, tokens, or
connection strings. Production log access must remain restricted; application
redaction reduces risk but does not replace operational access controls.

Console logs use JSON formatting so local and future centralized logging tools
can query the named fields.

## Validation Boundary

HTTP request-shape validation belongs at the API boundary. Business rules and
state transitions belong in Application and Domain. Product-specific request
validators will be introduced with their corresponding endpoints; no placeholder
product endpoint exists solely to demonstrate this infrastructure.
