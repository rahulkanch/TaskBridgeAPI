# Project Service Code Review

## TaskBridge Notification & Audit Service

**Reviewed Component:** Inherited AI-generated Project Service  
**Technology:** .NET 8, ASP.NET Core Web API, Entity Framework Core  
**Reviewer:** Kanchan Mankar  
**Review Date:** [ENTER DATE]  
**Review Status:** Completed  
**Initial Code Source:** GitHub Copilot generated output

---

## 1. Review Objective

The purpose of this review is to assess the initially generated Project
Service before integrating it with the new Notification & Audit Service.

The initial Project model and Project service were generated using the
mandatory low-effort prompt below and were saved without modification:

> Generate a Project model and a Project service with create, update status,
> get by team, and delete functions. Use a database.

The review focuses on:

- Layered architecture and separation of concerns
- Multi-tenant data isolation
- Authentication and authorization
- Database access and ORM usage
- Input validation
- Error handling
- Structured logging
- Sensitive-data exposure
- Asynchronous programming
- Testability
- Audit and notification integration risks
- Code quality and maintainability

---

## 2. Review Method

I performed the review using both GitHub Copilot and manual developer
judgment.

### Copilot-assisted review activities

1. Used **Ask Mode** with a role-based security-review prompt.
2. Used **#file** to provide the generated Project model and service as
   explicit context.
3. Used **@workspace** to compare the generated implementation with the
   repository architecture and custom instructions.
4. Used **/explain** or Inline Chat on selected methods whose database or
   authorization behavior required further analysis.
5. Asked Copilot to categorize findings by severity and recommend corrections.

### Manual review activities

1. Followed each create, query, update, and delete path.
2. Checked whether every database operation enforced `OrganisationId`.
3. Verified whether identity values came from authenticated claims or from
   untrusted request data.
4. Checked whether database, business, and HTTP responsibilities were mixed.
5. Examined deletion and status-update behavior for audit consistency.
6. Checked whether Copilot exposed persistence entities directly.
7. Reviewed logging for possible sensitive-data exposure.
8. Considered failure handling between project changes, audit recording, and
   notification creation.

Copilot's findings were treated as recommendations and were not accepted
without manual verification.

---

## 3. Executive Summary

The generated implementation provides basic Project operations, including
creation, status update, retrieval by team, and deletion. However, the initial
output is not suitable for production use without remediation.

The most significant review concerns are:

- [VERIFY: missing or incomplete organisation scoping]
- [VERIFY: request data controlling tenant identity]
- [VERIFY: database access mixed with business logic]
- [VERIFY: missing authorization]
- [VERIFY: missing validation]
- [VERIFY: unsafe hard-delete behavior]
- [VERIFY: insufficient error handling]
- [VERIFY: no reliable audit and notification consistency mechanism]

The required remediation is to introduce a layered Model, Repository, Service,
and Controller design; use Entity Framework Core for persistence; enforce
organisation-scoped access; add typed contracts and validation; implement
specific error handling and structured logging; and provide observable,
testable integration with the Notification & Audit Service.

---

## 4. Severity Definitions

### Critical

The issue could directly cause cross-tenant data disclosure, unauthorized
modification, loss of compliance evidence, or compromise of the application.

### High

The issue presents a significant security, correctness, data-integrity, or
operational risk and should be corrected before integration or deployment.

### Medium

The issue affects maintainability, reliability, testability, performance, or
standards compliance but may not immediately expose data.

### Low

The issue is primarily related to consistency, readability, documentation, or
minor code-quality improvements.

---

## 5. Detailed Review Findings

> Important: Keep only findings that are genuinely present in the generated
> code. Replace all `[VERIFY]` fields with the actual filename, class, and
> method. If a suggested issue does not exist, remove it and add another real
> issue.

---

### Finding 1: Project operations are not scoped by organisation

**Location:** `[VERIFY: ProjectService.cs, method name]`  
**Category:** Security  
**Severity:** Critical  
**Detected By:** `[ACTUAL: Ask Mode security review / Manual review]`

#### Issue

The database operation uses a project ID, team ID, or another identifier
without also requiring the authenticated user's `OrganisationId`.

Example pattern to look for:

```csharp
var project = await _dbContext.Projects
    .FirstOrDefaultAsync(p => p.Id == projectId);