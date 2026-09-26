**Author:** Daan Eggen  
**Date:** 05/09/2026  
**Version:** 1.0

---

# Project Overview: Smart Football Club Administration

## 1. Project Summary

For this project, I developed a working C# web prototype that brings member administration, contribution tracking and match planning together for a local football club. I completed the project through analysis, design, implementation and evaluation, documenting the decisions and results throughout the process. The result is a responsive dashboard supported by technical evidence, an operational plan and a reflection on my development.

## 2. Analysis and Design

I started by investigating the club's administrative problems, the people involved and the information needed to support their work. This helped me connect the required functionality to practical problems such as outdated member information, missed payments and planning conflicts. I documented these findings in <Project analysis>.

In <Deployment analysis>, I compared cloud and self-hosted deployment based on cost, control, complexity and learning value. I selected self-hosting as the intended deployment approach for the project. I then translated the requirements into an application structure, data model and main workflows in <Software design>. The accompanying <Infrastructure design> describes how the application could run on a Linux server with containers, secure connections, persistent storage and backups.

## 3. Implementation and Result

I built the prototype using ASP.NET Core and SQLite, with direct parameterized SQL through ADO.NET. The application generates demonstration data and allows users to view members, change team assignments, register contribution payments and create recorded reminders for overdue contributions. It also automatically schedules matches by checking player availability and excluding field conflicts with existing matches and training sessions.

The delivered functionality, technical structure and verification are described in <Implementation>. The solution compiled with zero warnings and zero errors, and database initialization successfully created the schema and demonstration records. I also supplied an HTTP request collection to make endpoint checks repeatable.

The completed result is a school prototype. The container-based PostgreSQL environment remains a deployment design, and production features such as authentication, authorization and actual email delivery are identified as further work.

## 4. Control, Monitoring and Research

I also considered how the application should be maintained after development. In <Operations control and monitoring>, I defined release checks, monitoring thresholds, backup requirements and recovery actions. This connects the implementation to the controls needed for reliable use.

In <Database optimization analysis>, I inspected database query plans and identified availability scans and contribution sorting as potential bottlenecks. I proposed suitable indexes and a measurable validation experiment. These recommendations still require testing with a representative dataset before their performance benefit can be confirmed.

Throughout the project, I used problem analysis, documentation research, design comparisons and technical inspection to support my decisions. <Exploratory research and reporting> explains these methods and shows how I connected findings to decisions while recording their limitations.

## 5. Professional and Personal Development

I organized the work into phases with clear deliverables and completion criteria. Git history and consistent documentation made the development process traceable. <Project organisation and communication> explains this approach, the project risks and the preparation for stakeholder feedback.

Finally, I evaluated my own way of working in <Reflection>. I identified concrete improvements in planning, responding to feedback, communicating expectations and continuing work when dependencies cause delays. Completing this project helped me connect technical development with responsibility for the process: understanding the problem, explaining my choices, delivering a working result and identifying what still needs improvement.
