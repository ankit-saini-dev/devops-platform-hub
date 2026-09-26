# API Conventions

## Status

The API foundation implements shared error handling and health endpoints.
Product endpoints, authentication, and API versioning remain planned.

## Error Responses

API failures are serialized from ASP.NET Core `ProblemDetails` as:

```text
application/json
```

Every shared error response contains a title, HTTP status, and type. For 400,
404, 409, and 422 responses, the API also includes a `detail` and `message`
with the handled exception message. Production 500 responses use the default
error message. Development 500 responses include exception detail for the local
developer who is debugging the API.

| Condition | HTTP status |
|---|---:|
| Invalid request data | 400 |
| Requested resource does not exist | 404 |
| Request conflicts with current state | 409 |
| Validation or unprocessable request | 422 |
| Unexpected failure | 500 |

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

The current refactor does not register a custom request-access logger.

Handled validation, conflict, and unprocessable-request failures are logged at
warning level with EventId `190201`. Unhandled failures are logged at error
level with EventId `190202` and retain the raw exception for server-side
debugging. The PostgreSQL readiness check records sanitized diagnostic details
when it converts a database failure to `503 Unhealthy`.

The logging sanitizer removes carriage-return and line-feed characters from
request values and redacts common password, token, authorization, and
PostgreSQL URI credential patterns when it is used. Production log access must
remain restricted.

## Validation Boundary

HTTP request-shape validation belongs at the API boundary. Business rules and
state transitions belong in Application. Product-specific validators will be
introduced with their corresponding endpoints; no placeholder product endpoint
exists solely to demonstrate this infrastructure.
