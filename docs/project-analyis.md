**Author:** Daan Eggen  
**Date:** 04/07/2026  
**Version:** 2.0

---

# Functional Design: Smart Football Club Administration

## 1. Purpose

This analysis investigates the problem of a local football club that currently manages member administration, contribution payments and match planning manually. The goal is to understand the club's situation, the involved users, the information that flows through the organization, and the processes that cause problems.

This document is positioned as a **functional design**: it describes users, information and intended behavior based on the assignment. The listed research methods describe the approach; they do not establish that stakeholder interviews or external validation took place. The original filename is retained so existing portfolio links continue to work.

## 2. Client and Context

The client is a local football club. The club needs better control over administrative processes that are now time-consuming and error-prone.

Important problems for the client are:

- Member data can become outdated or duplicated.
- Payment status is hard to track manually.
- Late contribution payments require manual follow-up.
- Team and training information may be spread across different documents.
- Match planning is difficult when player and field availability are not clearly visible.

The project focuses on understanding these problems and identifying which information is needed to improve them.

## 3. Target Audience

The main users are:

- **Board members / administrators**: responsible for members, payments and general administration.
- **Trainers**: need insight into teams, trainings, matches and player availability.
- **Members / players**: provide availability and are linked to teams, payments and matches.

The users are mostly club volunteers, so the administration process should be clear and not depend on technical knowledge.

## 4. Market and Related Products

Sports clubs often use spreadsheets, email, WhatsApp groups or general planning tools. These tools are easy to start with, but they do not keep all club information connected. For example, a spreadsheet with members does not automatically show whether a player is available for a match or whether contribution has been paid.

This shows that the club needs connected information rather than separate administration files.

## 5. Research Question

**How can the current member administration, contribution tracking and match planning processes of a local football club be understood and improved through better information management?**

Sub-questions:

1. Which users are involved in the current administration process?
2. Which information is needed for members, teams, payments, trainings and matches?
3. Which manual steps cause delays, mistakes or duplicated work?
4. Which information flows are needed between administrators, trainers and members?
5. Which constraints influence match planning, such as player and field availability?

## 6. DOT Framework Methods

| DOT area | Method                     | Reason                                                                         |
| -------- | -------------------------- | ------------------------------------------------------------------------------ |
| Field    | Stakeholder analysis       | To identify who uses or depends on the administration.                         |
| Field    | Process analysis           | To understand how member administration, payments and planning currently work. |
| Library  | Related product comparison | To learn how similar administration tools solve common club problems.          |
| Library  | Domain analysis            | To identify important football club concepts and terms.                        |
| Workshop | Requirements analysis      | To translate researched needs into clear problem areas.                        |

These methods are suitable because the project requires analysis of users, processes, information and existing solutions, not only technical implementation.

## 7. Process Analysis

### Member Administration

Member information is central to the club. If this information is managed manually, changes such as new members, stopped memberships or team changes can easily be missed.

Main findings:

- Member data should be reliable and up to date.
- Members are connected to teams, payments and availability.
- Administrators need a clear overview of member status.

### Contribution Payments

Contribution tracking is important for the club's financial administration. Manual tracking makes it easy to miss overdue payments.

Main findings:

- Each member needs a payment status.
- Due dates and paid dates are important.
- Overdue payments should be easy to identify.
- Reminders depend on accurate payment information.

### Teams and Trainings

Teams connect members, trainers, trainings and matches. If team information is spread across different places, trainers and administrators may work with different information.

Main findings:

- Team composition must be clear.
- Trainers need access to team-related information.
- Trainings must be linked to teams and fields.

### Match Planning

Match planning depends on multiple constraints. A match cannot be planned well without knowing which teams, players and fields are available.

Main findings:

- Player availability affects whether a team can play.
- Field availability affects when and where a match can take place.
- Planning conflicts can occur when information is incomplete.

## 8. Information Flow Analysis

| Information          | Source                          | Used by                           | Purpose                                    |
| -------------------- | ------------------------------- | --------------------------------- | ------------------------------------------ |
| Member details       | Administrator / member          | Administrator, trainer            | Registration and team overview             |
| Team assignment      | Administrator / trainer         | Trainer, planning process         | Knowing which players belong to which team |
| Payment status       | Administrator / payment records | Administrator                     | Tracking paid and overdue contributions    |
| Availability         | Member / trainer                | Trainer, planning process         | Checking whether players can participate   |
| Field availability   | Club planning                   | Administrator, planning process   | Avoiding field conflicts                   |
| Match information    | Planning process                | Trainers, members, administrators | Communicating schedules                    |
| Training information | Trainer / administrator         | Trainers, members                 | Organizing team activities                 |

The most important flow is that information entered by administrators, trainers and members must be consistent enough to support reliable planning and payment tracking.

## 9. Required Information

The analysis identifies the following required information groups:

- **Members**: identity, contact details, membership status and team.
- **Teams**: team name, category, trainer and players.
- **Payments**: member, contribution period, amount, due date, paid date and status.
- **Availability**: member, date/time and availability status.
- **Fields**: field name and availability.
- **Trainings**: team, trainer, date/time and field.
- **Matches**: teams, date/time, field and status.

These information groups are connected. For example, a member can belong to a team, have payment records and provide availability for matches.

## 10. Key Findings

- The club's main issue is not only storing data, but keeping related information consistent.
- Member administration, payment tracking and match planning influence each other.
- Administrators need reliable status information for members and payments.
- Trainers need accurate team and availability information.
- Match planning depends on both player availability and field availability.
- Manual administration increases the risk of missed updates, duplicated work and planning conflicts.

## 11. Functional acceptance criteria and delivered scope

| Use case | Acceptance criterion | Implementation / verification |
| --- | --- | --- |
| View administration | Show members, teams, contributions and matches | Dashboard and JSON; S01–S02 |
| Record payment | An unpaid contribution becomes paid and no longer overdue | I02, S03 |
| Change team | Assign or unassign a member and persist the choice | I03–I04, S04 |
| Generate reminders | Only overdue unpaid contributions; once per day | I05, S05; recorded, not emailed |
| Plan match | Enough players; no overlapping team/field booking | I06–I10, S06 |
| Preserve administration | Changes remain after restart | I12, S08 |

Member registration screens, training management, availability editing, authentication and actual email delivery remain desired functionality. Dummy data supplies members and availability; matches are generated by the planning action. See [implementation](implementation.md) and [test plan](test-plan.md).
