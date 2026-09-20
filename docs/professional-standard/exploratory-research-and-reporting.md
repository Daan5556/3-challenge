**Author:** Daan Eggen\
**Date:** 20/09/2026\
**Version:** 1.1

---

# Professional Practice: Exploratory Research and Reporting

## 1. Purpose

This document explains how exploratory research and reporting were applied during the football club administration project. It shows how questions, methods, evidence and decisions were connected instead of choosing a solution without investigation.

## 2. Research Question and DOT Approach

**Main research question:** How can a C# web prototype support consistent member administration, contribution tracking and conflict-free match planning for the football club described in the assignment, and what evidence is needed to evaluate its suitability?

The intended outcome is a prototype with traceable requirements, justified design choices and repeatable checks. Demonstrated improvement in a real club's administration would additionally require user evaluation; it cannot be inferred from implementation alone.

The DOT framework distinguishes five strategies: **Field** investigates the application context, **Library** studies existing knowledge, **Workshop** explores possible solutions, **Lab** tests their behaviour, and **Showroom** evaluates the work against existing work or expert judgement. A strategy explains the purpose of research; a method describes the activity used to obtain evidence. Strategies can be combined and revisited rather than treated as fixed project phases. [Source: ICT Research Methods, DOT framework](https://v2.ictresearchmethods.nl/dot-framework/).

Here, analysing the supplied case is Field document analysis, not a stakeholder interview. Architecture exploration and prototyping serve Workshop purposes. Executing `EXPLAIN QUERY PLAN` serves a Lab purpose because it observes database behaviour; it does not by itself measure response-time improvement. The mapping below describes how the existing investigations support the question, rather than claiming this complete plan was recorded before development.

## 3. Sub-Questions, Methods and Intended Outcomes

| ID and sub-question | DOT strategy and selected method | Why this method fits / intended outcome | Evidence, current answer and limitation |
| --- | --- | --- | --- |
| SQ1: Which users, information flows and constraints must the prototype support? | Field: document analysis of the supplied case and domain modelling. Proposed follow-up: user interview and workflow observation. | Extract the club context and planning rules into requirements; check interpretation with users later. | [Project analysis](../project-analyis.md) identifies administrators, trainers, members and connected data. Based on the assignment; no interview or observation is recorded. |
| SQ2: Which deployment approach fits the prototype's constraints? | Library: documentation research. Workshop: compare deployment alternatives against cost, control, maintenance and learning goals. | Use existing platform knowledge to justify a scoped deployment choice and identify responsibilities. | [Deployment analysis](../deployment-analysis.md) selects self-hosting. This is a qualitative choice; no measured cost comparison or operating deployment is demonstrated. |
| SQ3: How should the software and infrastructure be structured to implement those requirements? | Workshop: architecture modelling and prototyping, informed by the assignment's C# and no-ORM constraints. | Produce a data model, responsibility boundaries and an implementable prototype. | [Software design](../design/software-design.md), [infrastructure design](../design/self-hosted-infrastructure-design.md) and [implementation](../implementation.md). SQLite is implemented; the PostgreSQL/container environment is a design. |
| SQ4: What evidence shows the workflows behave correctly, and where might database growth cause problems? | Lab: build/startup checks and query-plan inspection. Planned: scenario tests and controlled before-and-after performance experiment. | Check executability, inspect query behaviour and define repeatable correctness and performance validation. | [Implementation](../implementation.md) records build/startup success and supplies requests, not a complete scenario-results log. [Database analysis](../database-optimization-analysis.md) reports scans and sorting, with an experiment still to run. |
| SQ5: What controls and external review are needed before the prototype can be considered suitable for use? | Workshop: risk analysis and recovery design. Planned Field: user walkthrough. Planned Showroom: expert review against explicit quality criteria. | Define monitoring and recovery criteria, then evaluate usability and technical suitability with people outside the implementation. | [Operations plan](../operations-control-and-monitoring.md) defines controls; [communication plan](project-organisation-and-communication.md) prepares review. No completed user walkthrough, expert review or recovery exercise is claimed. |

### Combining methods and using the results

For match planning, SQ1 establishes the availability and field-conflict requirements from the case. SQ3 translates them into a planner and data model. SQ4 checks whether the implementation behaves as intended and whether queries may become expensive. A planned SQ5 walkthrough should establish whether a trainer can understand the result and whether the assumptions match practice. These sources answer different parts of suitability; a fast query cannot establish user acceptance.

The next validation cycle should record: the requirement and expected result, the method and test conditions, the observation, the resulting decision, and a link to the changed artefact. For example, compare identical planning scenarios before and after adding an index. Keep the index only if results remain correct and the performance/storage trade-off meets the criteria in the database report. If a trainer disputes the availability rule, revise the requirement and repeat the affected checks.

### Current answer to the main question

The existing prototype brings the required information and workflows together using C# and direct SQL. The reports justify its structure and identify operational and performance risks. This supports technical feasibility within the supplied case. Suitability for actual club use remains provisional because user feedback, a complete behavioural test record and representative performance results are missing. Completing SQ4 and SQ5 would make that judgement stronger.

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

## 10. Feedback Repair and Sources

In response to the LO6/LO7 feedback, this revision adds an overarching research question, five sub-questions, a strategy/method mapping, evidence links and an explicit current answer. It distinguishes case analysis from direct user research and recorded technical findings from planned validation. The improvement is traceability of the research argument; it is not evidence that the outstanding experiments have been completed.

- [ICT Research Methods: DOT framework](https://v2.ictresearchmethods.nl/dot-framework/) — strategy definitions and combining methods.
- [ICT Research Methods: Field methods](https://ictresearchmethods.nl/field/) — document analysis and domain modelling.
- [ICT Research Methods: Showroom methods](https://v2.ictresearchmethods.nl/showroom/) — peer review and static program analysis.
- Project evidence and its original technical sources are linked in section 3.
