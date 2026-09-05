# Pull Request: TaskBridge Notification & Audit Service

## PR Title

feat: implement multi-tenant notification and immutable audit service

---

## 1. Summary

This pull request introduces the TaskBridge Notification & Audit Service and
remediates the inherited AI-generated Project Service.

The Project Service was initially created using the mandatory low-effort
GitHub Copilot prompt and preserved without modification before review. The
inherited implementation was then reviewed for architecture, security,
multi-tenant isolation, validation, persistence, error handling, logging, and
testability issues.

The remediated implementation introduces a layered architecture consisting of
models, repositories, services, controllers, typed contracts, and validators.
All project, audit, and notification operations are scoped to the authenticated
organisation.

The new Notification & Audit Service creates immutable audit entries and unread
notification records whenever a relevant project or milestone state changes.
Audit history can be queried by project with optional date-range and event-type
filters.

The implementation also supports the scope change for the
`MILESTONE_REOPENED` event and captures the actor's IP address as sensitive
audit metadata.

---

## 2. Business Context

TaskBridge is a multi-tenant B2B SaaS platform used by distributed engineering
teams. Project milestone changes must create two results:

1. An immutable audit entry recording who changed what and when.
2. A notification for every relevant project team member.

Clients must also be able to query project audit history using optional date
range and event-type filters.

The service must preserve organisation boundaries so that users cannot access
projects, audit records, or notifications belonging to another organisation.

---

## 3. Changes Included

### Project Service remediation

- Introduced Model, Repository, Service, and Controller layers.
- Replaced direct or unsafe database access with Entity Framework Core.
- Added typed request and response contracts.
- Added input validation.
- Added organisation-scoped repository operations.
- Added authenticated actor and organisation context.
- Added specific application exceptions.
- Added centralized error handling using `ProblemDetails`.
- Added structured logging.
- Added asynchronous database operations.
- Added `CancellationToken` support.
- Added documentation for public classes and methods.
- Preserved previous and new project state for audit integration.

### Notification & Audit Service

- Added immutable Audit Log model.
- Added Notification model.
- Added audit repository and service.
- Added notification repository and service.
- Added audit and notification API contracts.
- Added audit and notification validators.
- Added project-change orchestration.
- Added notification dispatch for relevant team members.
- Added audit-history filtering by date range.
- Added audit-history filtering by event type.
- Added unread-notification retrieval.
- Added mark-notification-as-read operation.
- Added support for the `MILESTONE_REOPENED` event.
- Added actor IP address to the audit contract.
- Added organisation isolation for all audit and notification operations.

### Documentation

- Added `.github/copilot-instructions.md`.
- Added `SPEC.md`.
- Added `REVIEW.md`.
- Added `IMPACT_ANALYSIS.md`.
- Added `PROMPTS.md`.
- Added `TOOL_STRATEGY.md`.
- Added `ARCHITECTURE.md`.
- Added this `PR_DESCRIPTION.md`.
- Updated `README.md` with the technology stack and usage instructions.

---

## 4. Architecture

The solution uses a layered feature-oriented architecture:

```text
HTTP Request
    |
Controller
    |
Validator
    |
Application Service
    |
Repository
    |
Entity Framework Core DbContext
    |
SQLite Database