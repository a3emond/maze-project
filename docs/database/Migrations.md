# Database Migrations in Entity Framework Core

## Overview
Database **migrations** in **Entity Framework Core (EF Core)** allow developers to **version control** database schema changes.  
They provide a way to modify the database structure **incrementally** without manually writing SQL scripts.

## Why Use Migrations?
- **Schema Versioning:** Keeps track of database changes over time.
- **Code-First Approach:** Allows defining and modifying the database schema directly in C# models.
- **Consistency:** Ensures that all environments (development, staging, production) have the same database structure.
- **Automatic Updates:** Generates the necessary SQL scripts to apply changes without manually altering the database.

## How Migrations Work
Migrations **track changes** in your entity models (`DbContext`) and generate **migration files** that define how the database should be updated.

The process involves:
1. **Detecting Changes** – EF Core compares the current model to the database schema.
2. **Generating Migration Files** – A new C# file is created with the changes.
3. **Applying Changes** – The migration updates the actual database schema.

## Common Migration Commands

### **1️⃣ Add a New Migration**
Creates a migration file that includes the latest model changes.

```sh
dotnet ef migrations add MigrationName
