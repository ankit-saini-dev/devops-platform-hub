# ADR 0006: Use JWT bearer tokens for local authentication

## Status

Accepted

## Context

The application needs a local authentication mechanism for its first protected
API endpoint. The backend and Angular client are separate applications, so the
authenticated request must carry identity and role claims across that boundary.
The project already has PostgreSQL for user and role persistence and ASP.NET
Core support for password hashing and bearer-token validation.

## Decision

Use locally issued, HMAC-SHA256 signed JWT bearer access tokens.

- Self-registration creates an active user with the `User` role.
- Login accepts a username or email and returns an access token on valid
  credentials.
- Passwords are stored only as hashes created by `IPasswordHasher<User>`.
- JWT issuer, audience, and lifetime are safe application configuration.
- The signing key is a secret supplied through .NET User Secrets locally and
  environment variables in CI or future deployment environments.
- The API validates issuer, audience, signing key, and expiry before allowing
  protected requests.
- Access tokens currently expire after 60 minutes. Refresh tokens, account
  verification, password reset, and external identity providers are deferred.

## Consequences

The Angular client can call protected API endpoints with a bearer token without
server-side session storage. JWT claims remain fixed until expiry, so a future
role change will apply when the user receives a new token. Revocation and
long-lived session behavior require a future, separately designed token
management feature.
