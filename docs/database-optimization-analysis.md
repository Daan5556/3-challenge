**Author:** Daan Eggen\
**Date:** 20/09/2026\
**Version:** 1.1

---

# Optimization Analysis: Database Queries

## 1. Purpose

This analysis investigates the database operations used by the football club prototype and identifies optimizations that become relevant when the amount of member, availability, payment and match data grows.

The goal is to improve performance based on measurements while preserving correct planning and administration behavior. This supports the learning outcome **Control, Monitoring and Optimization**.

## 2. Research Question

**Which database queries are likely to become bottlenecks, and how can they be optimized and monitored without making the prototype unnecessarily complex?**

Sub-questions:

1. Which queries currently perform full table scans or temporary sorting?
2. Which indexes match the filters and ordering used by the application?
3. How should improvement be measured before an optimization is accepted?
4. Which trade-offs do extra indexes introduce?

## 3. DOT Framework Methods

| DOT area | Method | Reason |
| --- | --- | --- |
| Lab | Query-plan inspection | To observe how SQLite executes important application queries. |
| Showroom | Static program analysis (self-inspection) | To connect query behavior to the implemented dashboard and planner. |
| Library | Database indexing principles | To select indexes that match filters, joins and ordering. |
| Lab | Before-and-after performance test | To validate improvements with representative data before release. |

Query-plan inspection observes execution behaviour and is therefore used as Lab research here. Static self-inspection relates the queries to the code; it is not evidence of an independent peer review. The before-and-after performance test below is planned, not completed. See the [overall research mapping](professional-standard/exploratory-research-and-reporting.md) for SQ4 and the [DOT method catalogue](https://v2.ictresearchmethods.nl/showroom/) for static program analysis.

## 4. Baseline

The prototype database inspected on 16/08/2026 contained:

| Table | Records |
| --- | ---: |
| Members | 16 |
| Availability | 224 |
| Contributions | 16 |
| Matches | 0 |

This dataset is too small for meaningful response-time conclusions. Query plans are still useful because they show which work will grow with the number of records.

## 5. Query-Plan Findings

### Player Availability

The planner counts available players for one team and date. `EXPLAIN QUERY PLAN` reported:

```text
SCAN a
SEARCH m USING INTEGER PRIMARY KEY (rowid=?)
```

SQLite scans the availability table and then looks up each matching member. Availability grows quickly because every member can have one record for every day. This query is therefore the highest optimization priority.

### Contribution Overview

The dashboard joins contributions to members and orders them by due date. The query plan reported:

```text
SCAN c
SEARCH m USING INTEGER PRIMARY KEY (rowid=?)
USE TEMP B-TREE FOR ORDER BY
```

The temporary B-tree means SQLite performs an additional sort. This is harmless for 16 rows but grows with the contribution history.

### Existing Strengths

- Primary and unique keys already create useful indexes for identifiers, email addresses and duplicate reminder prevention.
- All implemented user values are supplied through SQL parameters.
- The planner searches only 14 days, which places a clear upper bound on its loop.
- Database connections and readers are disposed after use.

## 6. Proposed Optimizations

| Priority | Change | Expected effect | Trade-off |
| --- | --- | --- | --- |
| High | Index availability by date, availability status and member | Reduces records inspected by the planner | Extra storage and write cost when availability changes |
| Medium | Index contributions by due date | Avoids or reduces sorting for the dashboard | Extra write cost for contributions |
| Medium | Index matches and trainings by field and start time | Speeds up field-conflict checks as schedules grow | Extra storage and maintenance |
| Low | Request only dashboard data needed for the current view | Reduces transfer and object creation with large histories | Requires pagination or separate queries |

Candidate SQLite indexes:

```sql
CREATE INDEX IF NOT EXISTS ix_availability_date_status_member
    ON availability(available_date, available, member_id);

CREATE INDEX IF NOT EXISTS ix_contributions_due_date
    ON contributions(due_date);

CREATE INDEX IF NOT EXISTS ix_matches_field_start
    ON matches(field_id, starts_at);

CREATE INDEX IF NOT EXISTS ix_trainings_field_start
    ON trainings(field_id, starts_at);
```

These are recommendations, not implemented changes. They should first be tested against representative data. When the deployment moves to PostgreSQL, its own query planner must be measured because SQLite results cannot be assumed to apply unchanged.

## 7. Validation Experiment

The optimization will be accepted using the following controlled experiment:

1. Generate at least 1,000 members, 365 availability records per member and a full contribution history in a separate test database.
2. Record the database size and query plans before adding indexes.
3. Execute each target query at least 100 times after a warm-up period.
4. Record median and 95th-percentile duration.
5. Add one candidate index at a time and repeat the same workload.
6. Verify that returned records and match-planning decisions are unchanged.
7. Retain only indexes that provide a measurable benefit for their storage and write cost.

| Query | Acceptance criterion |
| --- | --- |
| Available-player count | Uses an index and has a p95 below 100 ms on the test dataset. |
| Contribution overview | Does not require a temporary sort and has a p95 below 200 ms for one page. |
| Field-conflict check | Uses the field/time index and has a p95 below 100 ms. |
| Dashboard | 95% of complete requests finish below 500 ms. |

## 8. Monitoring After Release

- Record operation duration for dashboard loading, match planning and reminder generation.
- Review the slowest operations weekly during the prototype evaluation.
- Investigate when the p95 target is exceeded in two consecutive measurement periods.
- Track database size and record counts so performance changes can be related to data growth.
- Repeat query-plan inspection after schema changes or migration to PostgreSQL.
- Avoid logging personal member or payment data while collecting timings.

## 9. Conclusion

The current database is fast enough for its small dummy dataset, but query-plan inspection already reveals full scans and temporary sorting. Availability indexing is the first optimization to validate because this table grows fastest and directly affects automatic match planning. Contribution and schedule indexes are secondary improvements. No optimization should be accepted only on expectation: query plans, response-time percentiles, correctness and storage cost must be compared before and after the change.
