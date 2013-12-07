Decisions
==========

Highly concurrent claims based system for performing bespoke authorization checks.

Overview
=========

Self contained WebAPI based website that will use RESTful endpoints to describe claims that can then be configured to execute bespoke code for decision resolution.

Installation
-------------------------

Install the Decisions NuGet package into your API WebSite, or whichever Website you would like to host Decisions from. Install the Decisions.Contracts and Decisions.Utility NuGet packages into your Domain/Application layers and apply as set out in the Usage section below.

Once the NuGet packages are installed, you will need to implement policies and setup a Decisions.Config file then weave the implementations together with your chosen Inversion of Control strategy. You should implement a Decisions.Contracts.IResolver and apply it to Injector.Resolver on application startup.

Usage
-------------------------

### Basic

```c#
var decisionsService = Injector.Get<IDecisionsService>();
var context = new DecisionContext { Namespace = "Example", SourceId = "User", Role = "Role", Target = new { @id = 1 }};
var decision = await decisionsService.CheckAsync(context);
if(!decision) throw new UnauthorizedAccessException();
```

### Fluent

```c#
var decisionsService = Injector.Get<IDecisionsService>();
var context = DecisionContext.Create().Using("Example").As("User").Has("Role").On(new { @id = 1 });
var decision = await decisionsService.CheckAsync(context);
if(!decision) throw new UnauthorizedAccessException();
```

### Simplied Fluent

```c#
var decision = await DecisionContext.Create().Using("Example").As("User").Has("Role").On(new { @id = 1 }).Check();
if(!decision) throw new UnauthorizedAccessException();
```

### Lamda Simplied Fluent

```c#
var decision = await DecisionContext.Check(a => a.Using("Example").As("User").Has("Role").On(new { @id = 1 }));
if(!decision) throw new UnauthorizedAccessException();
```

### Lamda Simplied Fluent with Defaults

```c#
var decision = await DecisionContext.Check(a => a.Using("Example").Has("Role").On(new { @id = 1 }));
if(!decision) throw new UnauthorizedAccessException();
```