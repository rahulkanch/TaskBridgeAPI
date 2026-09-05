# TaskBridge Architecture

1. The Project Service owns project and milestone state, while the Notification
   & Audit Service records change evidence and creates user notifications.

2. The services integrate through an internal audit-event contract containing
   the event type, entity type, entity ID, project ID, actor user ID,
   organisation ID, previous state, new state, actor IP address, and UTC
   timestamp.

3. An inbound request first reaches the ASP.NET Core controller, which handles
   HTTP input, authentication context, and response generation.

4. Typed request models are validated before the controller calls the
   application service.

5. The application service contains business rules, validates milestone status
   transitions, and coordinates project, audit, and notification operations.

6. Repository interfaces isolate persistence concerns, and Entity Framework
   Core repositories perform asynchronous database operations.

7. Every project, audit, and notification repository operation is filtered by
   OrganisationId to enforce multi-tenant isolation.

8. The authenticated user ID and organisation ID are obtained from trusted
   claims rather than accepted solely from request-body values.

9. When project state changes, the service captures previous and new state,
   appends an immutable audit entry, and creates notifications for the relevant
   team members.

10. The audit repository exposes create and query operations only; update and
    delete operations are intentionally unavailable.

11. Audit history can be queried by project ID with optional date-range and
    event-type filters while remaining scoped to the authenticated
    organisation.

12. Typed request and response contracts prevent Entity Framework Core models
    and internal security fields from being exposed directly through the API.

13. Structured logging records operation identifiers and outcomes without
    logging authorization tokens, state snapshots, or actor IP addresses.

14. This layered architecture is appropriate for a multi-tenant B2B SaaS
    application because it separates HTTP, business, persistence, and security
    responsibilities and makes tenant-isolation rules independently testable.

15. The main trade-off is consistency between project changes, audit creation,
    and notification dispatch; the assessment implementation uses
    [ACTUAL CONSISTENCY APPROACH], while a production distributed deployment
    could use a transactional outbox with idempotent consumers.