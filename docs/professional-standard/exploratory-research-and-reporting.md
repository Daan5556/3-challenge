**Author:** Daan Eggen\
**Date:** 26/09/2026\
**Version:** 2.0

---

# Research Report: Football Club Administration

## 1. Problem, objective and scope

The [assignment](../../challenge-1.txt) describes a football club that struggles to keep member administration, contributions and match planning consistent when these are handled manually. Payments, team membership, player availability and field reservations affect one another. The research objective is to determine how a web prototype can connect these processes and what evidence supports its correctness and maintainability.

The assignment fixes two technical constraints: the application must use C# and must not use an ORM. These are requirements, not outcomes of a technology comparison. The investigated solution is a school prototype using demonstration data. No real club interviews, measured time savings or production deployment are established by the available evidence.

## 2. Main research question

**How can a C# web application support consistent member and contribution administration and availability-aware match planning for the football club in the assignment, and how can its correctness and maintainability be substantiated?**

“Consistent administration” means that payments and team changes persist, related records remain valid, and daily reminders are not duplicated. “Availability-aware planning” means that a proposed reservation has enough available players and no overlapping booking for its team or field. “Maintainability” is investigated through understandable code responsibilities, repeatable verification, deployment instructions and identified operational risks; long-term maintenance has not been measured.

The question connects the functional investigation to software design, deployment research, testing and database analysis. Its answer must distinguish demonstrated prototype behavior from suitability for actual club use.

## 3. Research design using DOT

DOT connects the application context, available knowledge and the solution being developed. It helps balance relevance to users with technical expertise, and broad exploration with certainty about specific behavior. The strategies used here are Field for understanding the case, Library for existing guidance, Workshop for exploring a solution, and Lab for testing it. Showroom would add comparison against external expertise or standards. Combining complementary methods strengthens the argument; the strategies are not sequential development phases. [Source: DOT framework](https://ictresearchmethods.nl/dot-framework/).

For this project, the application context is the supplied club case; available knowledge includes the documentation cited in the deployment and database investigations; the developed solution is the C#/SQLite prototype. This report reorganizes existing work retrospectively. It does not claim that this complete research plan was recorded before implementation.

| Sub-question | Selected strategy and activity | Why this activity is needed | Evidence |
| --- | --- | --- | --- |
| SQ1. Which users, data and rules must the application support? | Field: case-document analysis and domain modelling | Establish requirements before judging the solution | Assignment and functional design |
| SQ2. How can these rules be represented in working software without an ORM? | Workshop: architecture sketching, decomposition and prototyping | Explore responsibilities and demonstrate feasibility | Software design, C# code and SQL schema |
| SQ3. Which deployment approach fits the prototype and preserves its data? | Library: documentation research; Workshop: qualitative comparison of alternatives; Lab: publish and restart checks | Justify a hosting choice and test part of its implementation | Deployment comparison, deployment implementation and S08 |
| SQ4. To what extent do the implemented workflows satisfy the functional criteria? | Lab: unit, integration and HTTP system testing | Compare observed behavior with explicit expected results | Test runners and test report |
| SQ5. Which database and operational risks require further validation before wider use? | Lab: query-plan inspection; Workshop: improvement and recovery design | Identify likely growth and maintenance problems without claiming unmeasured benefits | Database investigation and operations plan |

The [Field catalogue](https://ictresearchmethods.nl/field/) supports document analysis and domain modelling. The [Workshop catalogue](https://ictresearchmethods.nl/workshop/) includes architecture sketching, decomposition, prototyping and multi-criteria decision making. The deployment comparison applies criteria qualitatively; it is not a weighted scoring experiment. The [Lab catalogue](https://ictresearchmethods.nl/lab/) includes unit, component, system and non-functional tests. The strategy labels describe why each activity contributes evidence, not merely when it happened.

## 4. Results and answers by sub-question

### SQ1. Required users, data and rules

**Method and material.** Analyse the supplied assignment and relate members, teams, contributions, availability, fields, trainings and matches. Record intended behavior and acceptance criteria in the [functional design](../project-analyis.md).

**Finding.** Administrators need connected member and payment information; trainers need team and planning information. A team change affects which players count toward planning. A payment changes whether a reminder is appropriate. A match must account for both player availability and occupied resources.

**Answer and decision.** Use one connected data model and dashboard. Make payment persistence, team assignment, reminder deduplication and planning constraints explicit acceptance criteria. Prioritize these workflows in the prototype.

**Limitation.** These needs are derived from the assignment. The functional design is a specification, not evidence of stakeholder interviews or observed real-world work. Member registration, availability editing and training-management screens remain outside the delivered interface.

### SQ2. Software structure and feasibility

**Method and material.** Explore the application through architecture diagrams, data modelling and a working prototype. Compare the [software design](../design/software-design.md) with the [implementation](../implementation.md) and its source links.

**Finding.** ASP.NET Core endpoints can render the dashboard and handle actions. `ClubDatabase` executes parameterized SQLite SQL and coordinates planning; `ClubRules` provides isolated rules; records hold dashboard and planning results. The SQL schema supplies foreign keys and a unique contribution/date constraint for reminders.

**Answer and decision.** C# with direct SQL implements the selected workflows without an ORM. Document the real responsibilities through C4 levels 1–4, and extract pure rules for independent tests. Keep SQLite for this prototype's simple setup.

**Limitation.** SQL and coordination remain combined in `ClubDatabase`. This demonstrates a workable small implementation, not that its architecture is optimal. Reminders are stored rather than emailed. Authentication is absent, and concurrent reservations are not protected against races.

### SQ3. Deployment choice and data preservation

**Method and material.** Compare cloud and self-hosting using the criteria in [deployment analysis](../deployment-analysis.md): cost, control, complexity, maintenance and learning value. Translate the choice into [deployment configuration and instructions](../deployment-implementation.md), then check the published application and restart behavior.

**Finding.** The qualitative comparison favors self-hosting for control and learning within this assignment. The implemented configuration is one ASP.NET Core container with SQLite on a persistent volume. A configurable database path separates stored data from the application files. Published-process system test S08 preserves a payment and match after restart; Compose configuration validation succeeds.

**Answer and decision.** Provide a repeatable publish procedure and Docker Compose configuration with persistent storage. Record backup and rollback procedures as operational requirements.

**Limitation.** No measured hosting-cost comparison was performed. Container build/start, volume persistence, backup restoration and deployment to a real server have not been demonstrated. Published-process restart evidence does not prove container operation. PostgreSQL is not the delivered deployment database.

### SQ4. Functional correctness

**Method and material.** Run the scenarios encoded in the [C# test runner](../../tests/Program.cs) and [HTTP system runner](../../scripts/system-tests.py) using isolated temporary databases and the published HTTP application. Compare each observation with its expected result; preserve the execution outcome in the [test report](../test-report.md).

**Finding.** The recorded run on 26/09/2026 passed six unit checks, twelve SQLite integration checks and eight HTTP system checks. These cover overdue-date boundaries, player thresholds, persisted payments and memberships, reminder deduplication, team/field conflicts, invalid planning input, HTML escaping and restart persistence. Publishing succeeded; vulnerability auditing produced a network-related NuGet warning.

**Answer and decision.** The evidence supports the tested sequential workflows. Inspection also exposed a planner defect: a free second field could allow the same team to be booked twice simultaneously. The query was changed to check team as well as field conflicts, and I07–I09 exercise overlapping match/training scenarios.

**Limitation.** The 26 named checks are not a code-coverage percentage or proof of complete correctness. They do not establish visual usability, accessibility, race-free concurrent planning or full security. The unit tests cover extracted rules; SQL behavior is checked at integration level.

### SQ5. Growth and operational risks

**Method and material.** Review the historical query-plan observations in [database optimization analysis](../database-optimization-analysis.md) and connect them to recovery and monitoring requirements in the [operations plan](../operations-control-and-monitoring.md).

**Finding.** The database report records an availability-table scan and temporary sorting of contributions on a small baseline of 16 members and 224 availability records. These indicate work that may grow; they do not establish unacceptable response times. Proposed indexes and a larger controlled experiment are documented. The operations plan identifies monitoring, backups and recovery checks, but these controls are not a completed operating service.

**Answer and decision.** Prioritize a representative before-and-after indexing experiment and a backup/restore rehearsal before wider use. Preserve functional results while evaluating query plans, timings, storage and write cost. Treat the old query-plan observations as a baseline to repeat against the revised planner.

**Limitation.** No measured performance improvement or verified recovery is established. The older operations plan includes proposed controls and earlier infrastructure assumptions; the deployment implementation is authoritative for the current SQLite setup.

## 5. Combining evidence and evaluating validity

The planning investigation demonstrates the connection between the activities. SQ1 supplies the requirement from the case. SQ2 turns it into a data model and implementation. SQ4 tests bookings and leads to a correction when the implementation permits overlapping team reservations. SQ5 asks whether the corrected queries remain practical when the database grows. Each result answers a different part of the main question.

Agreement between the requirements, code and tests provides technical support, but all three originate within this project. A mistaken interpretation of the club's needs could therefore survive the tests. A planned trainer walkthrough should evaluate whether the chosen availability model and planning result fit actual practice. A usability test belongs to Lab when tasks and observations are evaluated under defined conditions; an interview about everyday practice serves Field research. External technical comparison or expert assessment could add Showroom evidence, but no such completed review is claimed here.

For the next cycle, record the question, method, conditions, observation, decision and affected artifact together. Repeat relevant checks after a change. Preserve unsuccessful findings as well as successful results; a proposed index should be rejected if its measured cost outweighs its benefit.

## 6. Conclusion: answer to the main research question

The prototype supports consistent administration by connecting members, teams, contributions and reservations in one SQLite database, applying constraints and parameterized updates, and presenting the workflows through a C# web dashboard. Availability-aware planning checks player counts and overlapping team/field bookings before saving a reservation. The recorded unit, integration and HTTP checks substantiate these behaviors within the tested sequential scenarios.

Understandable code diagrams, repeatable checks, deployment files and documented recovery requirements provide a basis for maintaining the prototype. They do not establish long-term maintainability or readiness for actual club use. User evaluation, representative performance measurements, concurrency protection and verified container deployment/recovery remain necessary. The research therefore supports technical feasibility and tested prototype behavior, while leaving practical suitability provisional.

## 7. Follow-up research

| Remaining question | Proposed method and evidence | Decision it should support |
| --- | --- | --- |
| Can a trainer and administrator complete and understand the main workflows? | Task-based usability test; record completion, errors, misunderstandings and feedback | Revise interface and requirements where observed problems occur |
| Do the candidate indexes improve the revised queries? | Controlled experiment defined in the database report, using identical data before/after | Retain only improvements supported by measurements and unchanged results |
| Does the deployment preserve and recover data? | Container recreation and backup/restore test on a separate volume | Accept deployment only after stored records survive and recovery is verified |
| Are technical risks adequately addressed? | External review against explicit design and quality criteria | Prioritize remaining defects and architectural changes |

These activities are proposed work. A screencast can demonstrate the product but does not replace user evaluation or a test-results record.

## 8. Sources and evidence status

The supplied [Canvas research page](https://fhict.instructure.com/courses/15970/pages/onderzoek) could not be accessed during this revision. Its contents have not been verified. The public DOT explanation and method catalogues below were consulted on 26/09/2026; no claim is made that they reproduce all course-specific instructions.

- [ICT Research Methods: DOT framework](https://ictresearchmethods.nl/dot-framework/).
- [ICT Research Methods: Field methods](https://ictresearchmethods.nl/field/).
- [ICT Research Methods: Workshop methods](https://ictresearchmethods.nl/workshop/).
- [ICT Research Methods: Lab methods](https://ictresearchmethods.nl/lab/).

Project evidence is linked beside each sub-question. Historical query-plan findings are attributed to the database report; they were not rerun for this document revision. Test outcomes are attributed to the dated test report. This revision reorganizes the research argument and updates its conclusions to the current evidence; it does not create additional experimental results.
