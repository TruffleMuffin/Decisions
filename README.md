Decisions
==========

A Claims based model for resolving bespoke authorization/security operations. Provides a layer of abstraction between authorization details and calling the operation. Thus, allowing the same operation to resolve differently depending on setup of Decisions not a change to your codebase. Useful when you have a lot of private or security driven parts to your application and want to have flexibility to change your security model with minimal risk and time involved.

Overview
-------------------------

Self contained WebAPI application that uses RESTful endpoints to describe claims that can then be configured to execute bespoke code for decision resolution. 

Decisions has three key elements. A Policy, a Environment and a Decision. Each Decision is an Expression involving one or more Policies that resolves to a boolean True or False. Each Policy represents an element of the Decision and can use one or more Environments to resolve itself to a boolean True or False.

Decisions
-------------------------

A Decision is a simple string that is described as part of the Decisions.config or like file you initialize Decisions with. Example of Decisions are:

* True
* False
* True OR False
* True AND False
* True AND (True OR False)
* !False

You can use Policies as part of Decisions, there are also some short hand conventions. The following can also be used:

* . instead of AND
* + instead of OR

The Decision has a limited grammer as of 1.1.0 in order to ensure it is successfully resolved. The following applies

decision : = policy | "(" policy operator policy ")" 

operator :=  "AND" | "OR" | "." | "+"

policy := "True" | "False" | a string that appears in the policy list

Environments
-------------------------

A Environment is some object that can be retrieved by the information available in the DecisionContext. The object can then be used within a Policy to help resolve it. You will need to implement IEnvironmentProvider in order to provide Environments in your Policies. The following is a simple example that uses the SourceId as a Current Username to resolve further details about that User.


```c#
public class EnvironmentProvider : DefaultEnvironmentProvider 
{
    public override Task<dynamic> GetAsync(string alias, DecisionContext context)
    {
        if (alias == EnvironmentKeys.CURRENT_USER)
        {
            return Task.FromResult((object)userService.GetUser(context.SourceId));
        }
    }
}
```

Policies
-------------------------

A Policy is a atomically resolvable operation which alone or with other Policies resolves a Decision. Implementing one allows you to provide custom logic while executing a Decision. The Policy can then be registered with a PolicyProvider like the Example implementation to be executed in a Decision. Recommended approach for implementing a Policy is outlined below.

```c#
public class IsCurrentUserPolicy : AbstractPolicy
{
    public override bool Decide(DecisionContext context)
    {
        var currentUser = GetEnvironment(EnvironmentKeys.CURRENT_USER, context) as User;
        return context.Target.Id == currentUser.Id;
    }
}
```

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
