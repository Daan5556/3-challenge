**Author:** Daan Eggen  
**Date:** 04/07/2026  
**Version:** 1.0

---

# Deployment Analysis: Cloud vs Self-Hosted

## 1. Purpose

This analysis compares cloud deployment and self-hosted deployment for the football club administration project. The goal is to decide which option fits the project best, based on cost, control, complexity and learning value.

## 2. Research Question

**Should the application be deployed to a cloud platform or hosted on a self-managed server?**

Sub-questions:

1. Which deployment option is simpler for a school prototype?
2. Which option gives the most control over the application and database?
3. Which option has the lowest risk of unexpected costs?
4. Which option best supports learning about deployment and infrastructure?

## 3. DOT Framework Methods

| DOT area | Method | Reason |
| --- | --- | --- |
| Library | Documentation research | To compare cloud and self-hosted deployment using official sources. |
| Workshop | Trade-off analysis | To compare both options against the project needs. |
| Field | Project context analysis | To check which option fits an individual school assignment. |

## 4. Option Comparison

| Aspect | Cloud deployment | Self-hosted deployment |
| --- | --- | --- |
| Setup | Easier when using managed services such as Azure App Service. | Requires setting up and maintaining the server environment. |
| Cost | Can start free, but paid services may be needed for production use, storage or database hosting. | Can be low-cost if existing hardware or a small VPS is used. |
| Control | Less control over the underlying infrastructure. | More control over server, database, configuration and deployment process. |
| Maintenance | Cloud provider manages much of the platform. | The developer is responsible for updates, backups and uptime. |
| Learning value | Good for learning managed cloud platforms. | Good for learning servers, containers, networking and deployment fundamentals. |
| Fit for this project | Useful, but possibly more than needed for a prototype. | Fits a small prototype where control and cost predictability matter. |

## 5. Key Findings

- Cloud deployment is convenient because the platform handles much of the infrastructure.
- Cloud services can introduce account setup, pricing and service configuration complexity.
- Free cloud tiers are useful for testing, but are often limited and not always intended for production workloads.
- Self-hosting gives more direct control over the application, database and server environment.
- Self-hosting matches the scale of this project because the application is a school prototype with limited users.
- Using tools such as Docker Compose can keep a self-hosted setup understandable by managing the application stack in one configuration.

## 6. Conclusion

For this project, I will choose **self-hosted deployment**.

This is the better choice because the application is a small school prototype, not a production system that needs automatic scaling or enterprise availability. Self-hosting gives more control over the server and database, keeps costs more predictable, and provides useful learning about how the application actually runs outside the development environment.

Cloud deployment would be a good option for a larger or public production version, but for this project self-hosting fits the scope better.

## 7. Sources

- Microsoft Azure App Service overview: https://learn.microsoft.com/en-us/azure/app-service/overview
- Microsoft Azure App Service pricing: https://azure.microsoft.com/en-us/pricing/details/app-service/windows/
- Docker Compose documentation: https://docs.docker.com/compose/

## 8. Implementation follow-up (26/09/2026)

The hosting comparison above records the original decision. The delivered configuration now consists of one ASP.NET Core container with SQLite on a persistent volume. See [deployment implementation](deployment-implementation.md) for the actual Dockerfile, Compose configuration, publish procedure and verification status. No public server deployment is claimed.
