# Insurance Management System

Web application for managing insured persons and their insurance policies.

The project was created as a portfolio project focused on ASP.NET Core MVC,
Entity Framework Core, ASP.NET Core Identity, relational databases,
authorization, validation and automated testing.

## Features

### Administrator

Users with the `Admin` role can:

- view all insured persons
- create new insured persons
- edit insured person data
- delete insured persons
- view insured person details
- view all insurance policies
- create new insurance policies
- edit insurance policies
- delete insurance policies
- view insurance policy details

When an administrator creates a new insured person, the application also creates
an ASP.NET Core Identity account and assigns the `Poistenec` role.

When the insured person's email address is changed, the email and username used
for authentication are updated as well.

When an insured person is deleted, their user account and related insurance
policies are also deleted.

### Insured Person

Users with the `Poistenec` role can:

- view their own personal data
- view their own insurance policies
- view details of their own insurance policies

Insured persons cannot access the list of other insured persons or administrative
insurance management pages.

The application also verifies insurance ownership on the server, preventing a
user from accessing another person's insurance policy by manually changing the
ID in the URL.

## Authentication and Authorization

Authentication is implemented using ASP.NET Core Identity.

The application uses two roles:

- `Admin`
- `Poistenec`

Public user registration is disabled. Accounts for insured persons can only be
created by an administrator.

After successful login:

- administrators are redirected to the insured persons list
- insured persons are redirected to their personal data page

The relationship between `ApplicationUser` and `Poistenec` is configured as
one-to-one. An administrator account does not need to be linked to an insured
person.

## Insurance Management

Each insured person can have multiple insurance policies.

When creating a new insurance policy, the administrator can search for an insured
person by name, surname or email using an autocomplete field.

The selected insured person is stored internally through `PoistenecId`.

After an insurance policy has been created, its owner cannot be changed through
the edit form.

## Validation

The application validates user input both on the client and on the server.

Validation includes:

- required fields
- email address validation
- duplicate email validation
- password rules
- positive insurance amount
- insurance date validation
- insurance start date cannot be in the past when creating a new policy
- insurance end date must be later than the allowed minimum date
- insurance end date cannot be earlier than the start date

Password requirements include:

- at least 6 characters
- lowercase letter
- uppercase letter
- digit
- non-alphanumeric character

## Database Model

Main relationships:

```text
ApplicationUser 1 ─── 0..1 Poistenec

Poistenec 1 ─── * Poistenie
```

`ApplicationUser` is used for authentication and authorization.

`Poistenec` stores the insured person's personal information.

`Poistenie` stores insurance policy information and references its insured person
through `PoistenecId`.

Entity Framework Core migrations are used to create and update the database
schema.

## Technologies

- C#
- .NET 10
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server LocalDB
- Razor Views
- Bootstrap
- HTML
- CSS
- JavaScript
- xUnit
- ASP.NET Core `WebApplicationFactory`
- SQLite in-memory database for integration tests

## Testing

The project contains automated unit and integration tests.

At the current stage, the solution contains **18 automated tests**.

The tests cover areas such as:

- insurance date validation
- insurance amount validation
- anonymous access protection
- role-based authorization
- administrator access
- insured person access restrictions
- access to own insurance policy
- protection against access to another user's insurance policy
- disabled public registration
- creation of an insured person and Identity account
- duplicate email handling
- deletion of insured persons, accounts and related insurance policies
- synchronization of email changes with the Identity account
- validation of insurance start and end dates

Integration tests use `WebApplicationFactory` and an isolated SQLite in-memory
database.

## Security

The application includes several security measures:

- ASP.NET Core Identity authentication
- role-based authorization
- server-side ownership checks
- CSRF protection using anti-forgery tokens
- disabled public registration
- restricted administrative controllers
- database constraints for user-to-insured-person relationships
- server-side validation in addition to client-side validation

## Requirements

- .NET 10 SDK
- SQL Server LocalDB or SQL Server
- Visual Studio with ASP.NET Core development support

## Setup

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Check the `DefaultConnection` connection string in `appsettings.json`.
4. Open Package Manager Console.
5. Run:

```powershell
Update-Database
```

6. Run the application.

The database will be created according to the Entity Framework Core migrations.

## Demo Administrator Account

The application contains a demo administrator account for portfolio presentation.

```text
Email: admin@admin.sk
Password: Admin123!
```

This account is intended only for demonstration purposes.

## Screenshots

Screenshots of the application can be added here before publishing the repository.

Suggested screenshots:

- home page
- administrator insured persons list
- insured person details
- insurance creation with autocomplete
- insurance list
- insured person's personal page
- login page

## Author

Krištof Kostolányi
