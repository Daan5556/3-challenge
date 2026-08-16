**Author:** Daan Eggen  
**Date:** 16/08/2026  
**Version:** 1.0

---

# Professional Practice: Exploratory Research and Reporting

## 1. Purpose

This document explains how exploratory research and reporting were applied during the football club administration project. It shows how questions, methods, evidence and decisions were connected instead of choosing a solution without investigation.

## 2. Research Approach

The project used several small investigations because one method could not answer every type of question.

```mermaid
flowchart LR
    Question[Research question] --> Method[Select DOT method]
    Method --> Evidence[Collect evidence]
    Evidence --> Compare[Compare options or results]
    Compare --> Decision[Make a scoped decision]
    Decision --> Validate[Validate in design or implementation]
    Validate --> Report[Report result and limitations]
```

The DOT Framework was used to distinguish research in the problem context, existing knowledge and practical experimentation.

## 3. Research Activities

| Research topic | Question | Method | Result |
| --- | --- | --- | --- |
| Club administration | Which users, processes and information are involved? | Field-oriented stakeholder and process analysis | Board members, trainers and members were identified with their required information flows. |
| Deployment | Does cloud or self-hosted deployment fit the prototype? | Library research and trade-off analysis | Self-hosting was selected for control, predictable cost and learning value. |
| Software structure | How can direct SQL remain maintainable? | Workshop design and architecture comparison | Responsibilities were separated into the web interface, application behavior, models and data access. |
| Infrastructure | How can the application be deployed and recovered? | Design exploration and risk analysis | A Docker Compose design with restricted networking, persistence, backups and rollback was created. |
| Database performance | Which queries may become bottlenecks? | Query-plan inspection and code review | Availability scans and contribution sorting were identified as optimization candidates. |
| Operational quality | How can failures be detected and controlled? | Risk and threshold analysis | Health, performance, backup and recovery criteria were defined. |

## 4. Example of Evidence-Based Exploration

The database optimization investigation demonstrates the research process:

1. The implemented SQL was reviewed to select important queries.
2. The database contents were counted to establish the size of the baseline.
3. SQLite `EXPLAIN QUERY PLAN` was used to inspect execution behavior.
4. A full availability scan and temporary contribution sort were observed.
5. Candidate indexes were proposed based on the actual filter and ordering columns.
6. The indexes were not presented as proven improvements because the current dataset is too small.
7. A larger before-and-after experiment and measurable acceptance criteria were defined.

This is exploratory research because the outcome was allowed to depend on observed evidence. It also separates a finding, a recommendation and a validated result.

## 5. Decision Traceability

| Decision | Supporting evidence | Limitation or condition |
| --- | --- | --- |
| Build a C# web application | Explicit challenge requirement | Other platforms were not relevant options. |
| Use direct ADO.NET SQL | ORM frameworks are prohibited by the assignment | More manual mapping and SQL maintenance are required. |
| Use SQLite for the prototype | Simple local setup and reproducible dummy data | PostgreSQL remains the intended self-hosted deployment database. |
| Use a layered design | Business rules and data access need independent validation | The small prototype currently combines some coordination in `ClubDatabase`. |
| Prefer self-hosted deployment | Comparison of control, complexity, cost and learning value | High availability is not provided by one server. |
| Prioritize availability indexing | Query plan shows a scan on the fastest-growing dataset | Benefit must still be measured with representative data. |

Recording limitations prevents a decision from appearing more certain or complete than the evidence supports.

## 6. Reporting Standard

The project documents use a consistent structure:

- Author, date and version identify the document.
- Purpose and research questions explain why the work was performed.
- Methods show how information was collected or compared.
- Tables and diagrams communicate relationships compactly.
- Findings are separated from decisions and future recommendations.
- Validation criteria make results testable.
- Conclusions answer the purpose without introducing new claims.
- Sources are listed when external documentation supports an analysis.

Technical reports use precise terms such as parameterized SQL, p95 response time and rollback only where these help the intended reader. Portfolio summaries use shorter, non-technical language so an assessor can understand the professional activity before opening the detailed evidence.

## 7. Quality Review Checklist

Before a document is submitted, I check:

| Check | Acceptance criterion |
| --- | --- |
| Relevance | The document directly supports the challenge or a learning outcome. |
| Accuracy | Claims match the current code, measurements or cited design status. |
| Traceability | Important decisions can be linked to requirements or evidence. |
| Readability | Headings, tables and diagrams have a clear purpose. |
| Reproducibility | Commands, criteria or scenarios allow important results to be repeated. |
| Transparency | Assumptions, limitations and planned work are identified. |
| Privacy | Reports and screenshots contain no real member or secret data. |
| Consistency | Terminology, dates and formatting match the other portfolio documents. |

## 8. Reflection

Using multiple research methods improved the project because technical choices were related to the club context, assignment constraints and observed implementation behavior. Query-plan inspection was especially useful because it replaced a general assumption about performance with a specific finding.

The research can be strengthened by collecting primary stakeholder feedback and executing the proposed performance experiment with a representative dataset. Source references should also be expanded when later reports depend on external security, accessibility or deployment standards.

## 9. Conclusion

Exploratory research was applied through problem analysis, literature and documentation research, trade-off analysis, design exploration and technical inspection. Professional reporting made the resulting decisions, evidence and limitations traceable. The next improvement is to add primary stakeholder feedback and measured before-and-after optimization results to the existing evidence.
