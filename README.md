Decisions
==========

Highly concurrent claims based system for performing bespoke authorization checks.

Overview
=========

Self contained WebAPI based website that will use RESTful endpoints to describe claims that can then be configured to execute bespoke code for decision resolution. Please forgive the lack of detailed documentation, however if you read below you can get a brief introduction into how this project should be used which you can then see in action using the Test and Example projects.

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

### Lamda Simplified Fluent

```c#
var decision = await DecisionContext.Check(a => a.Using("Example").As("User").Has("Role").On(new { @id = 1 }));
if(!decision) throw new UnauthorizedAccessException();
```

### Lamda Simplified Fluent with Defaults

```c#
var decision = await DecisionContext.Check(a => a.Using("Example").Has("Role").On(new { @id = 1 }));
if(!decision) throw new UnauthorizedAccessException();
```

Policies
-------------------------

Policies are a building block of Decisions. Implementing one allows you to provide custom logic while executing a Decision based on the current Context. The Policy can then be registered with a PolicyProvider like the Example implementation to be executed in a Decision.Recommended approach for a Policy is outlined below.

```c#
public class AlphaPolicy : AbstractPolicy
{
    public override bool Decide(DecisionContext context)
    {
    	// You should implement your custom logic here
        return context.Target.id == 1;
    }
}
```
