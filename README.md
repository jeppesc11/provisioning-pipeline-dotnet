# ProvisioningPipeline 🚀

A dynamic and extensible engine for building provisioning pipelines, focused on Microsoft 365 (M365) scenarios, but adaptable for any provisioning task.

## Features

- 🛠️ **Flexible Middleware Architecture:** Compose complex provisioning flows using middleware.
- ⚡ **Fast and Dynamic:** Build and modify pipelines with minimal code.
- 🔗 **Extensible Hooks:** Add custom logic and notifications with hooks.
- 💡 **Fine-grained Step/Event Control:** Use pipeline options for granular handling of step events and errors.

## Table of Contents

- [Overview](#overview)
- [Core Concepts](#core-concepts)
- [Builder Methods & API](#builder-methods--api)
- [ProvisioningPipelineBuilder\<T\> Method Reference](#-provisioningpipelinebuildert-method-reference)
- [Example: Building a Pipeline](#example-building-a-pipeline)
- [Advanced Topics](#advanced-topics)

---

## Overview

ProvisioningPipeline helps you build robust provisioning workflows for M365 or other systems using a middleware approach, similar to web frameworks. Define each provisioning step as middleware and chain them together for flexibility and reusability.

---

## Core Concepts

- **Step:** A unit of provisioning work (e.g., create a site, assign a license).
- **Middleware:** Cross-cutting logic that wraps pipeline execution (e.g., logging, retry, timing).
- **Hook:** Code that runs before/after steps or on specific events (e.g., notifications).
- **Context:** Passes the model and shared state between steps.

---

## Builder Methods & API

- **AddStep:** Add a step to the pipeline.
- **AddStepWhen:** Add a step with a runtime condition.
- **AddParallelSteps:** Add steps to run in parallel.
- **UseMiddleware:** Add middleware logic.
- **Build:** Build the final pipeline with options.

---

## 📦 ProvisioningPipelineBuilder\<T\> Method Reference

| Method | Description | Parameters | Returns | Usage Example |
|--------|-------------|------------|---------|--------------|
| **AddStep** | Registers a single step to always be executed in the pipeline (shorthand for AddStepWhen with `true` condition). | `IProvisioningStep step`: The step to add. | `ProvisioningPipelineBuilder<T>` (for chaining) | `builder.AddStep(new Step1());` |
| **AddStepWhen** | Registers a step that is only executed if a condition is met at runtime. | `Func<ProvisioningContext<T>, bool> condition`: Predicate to evaluate.<br>`IProvisioningStep step`: The step to add. | `ProvisioningPipelineBuilder<T>` | `builder.AddStepWhen(ctx => ctx.Model.Age > 18, new Step2());` |
| **AddParallelSteps** | Registers multiple steps to be executed in parallel as a group. | `params IProvisioningStep[] steps`: Steps to run in parallel. | `ProvisioningPipelineBuilder<T>` | `builder.AddParallelSteps(new Step3(), new Step4());` |
| **UseMiddleware** | Adds a middleware component to the pipeline, wrapping or augmenting step execution. | `IProvisioningMiddleware<T> middleware`: The middleware instance to add. | `ProvisioningPipelineBuilder<T>` | `builder.UseMiddleware(new LoggingMiddleware<T>());` |
| **Build** | Builds and returns a configured pipeline. Accepts an optional configuration callback for pipeline options. | `Action<ProvisioningPipelineOptions<T>>? configure`: Optional configuration for options and hooks. | `ProvisioningPipeline<T>` | `var pipeline = builder.Build(opts => { opts.ContinueOnError = true; });` |

---

## ⚙️ ProvisioningPipelineOptions<T>

The `ProvisioningPipelineOptions<T>` class lets you customize how your provisioning pipeline behaves, handles errors, and reacts to step events.

| Property                | Type                                         | Description                                                                                  |
|-------------------------|----------------------------------------------|----------------------------------------------------------------------------------------------|
| `OnStepStart`           | `Action<IProvisioningStep>`                  | Called before a step starts. Great for logging or custom pre-step logic.                     |
| `OnStepComplete`        | `Action<IProvisioningStep>`                  | Called after a step completes. Use for post-step actions or cleanup.                         |
| `OnError`               | `Action<IProvisioningStep, Exception>`       | Called if a step throws an exception. Useful for logging, notifications, or custom recovery. |
| `ContinueOnError`       | `bool`                                       | If true, the pipeline continues even after errors; otherwise, it stops on the first error.   |
| `Hooks`                 | `List<IProvisioningHook<T>>`                 | General hooks that apply to all steps.                                                       |
| `HookRegistrations`     | `List<(Predicate<IProvisioningStep>, IProvisioningHook<T>)>` | Internal. Allows conditional hooks for specific steps based on a predicate.                  |

### 🔗 Registering Conditional Hooks

Attach a hook that only applies to steps matching a condition:

```csharp
options.Hooks.AddForStep<Step1, TestProvisioningModel>(new NotificationHook<TestProvisioningModel>());
```

---
## Example: Building a Pipeline

```csharp
var pipeline = new ProvisioningPipelineBuilder<TestProvisioningModel>()
    .AddStep(new Step1("YoYoYo"))
    .AddStep(new Step2())
    .AddStepWhen(ctx => ctx.Model.Age > 18, new ConditionalStep())
    .AddStep(new Step3())
    .AddParallelSteps(
        new Step4(), 
        new Step5()
    )
    .UseMiddleware(new LoggingMiddleware<TestProvisioningModel>())
    .UseMiddleware(new RetryMiddleware<TestProvisioningModel>(maxRetries: 3, delay: TimeSpan.FromSeconds(2)))
    .UseMiddleware(new TimingMiddleware<TestProvisioningModel>())
    .Build(options => {
        options.OnStepStart = step => Console.WriteLine($"[START] {step.Name}");
        options.OnStepComplete = step => Console.WriteLine($"[END] {step.Name}");
        options.OnError = (step, ex) => Console.WriteLine($"[ERROR] {step.Name}: {ex.Message}");
        options.Hooks.AddForStep<Step1, TestProvisioningModel>(new NotificationHook<TestProvisioningModel>());
        options.ContinueOnError = false;
    });
```

---

## Advanced Topics

- **Custom Middleware:** Implement `IProvisioningMiddleware<TModel>` to intercept and augment pipeline flow.
- **Custom Hooks:** Implement `IProvisioningHook<TModel>` for custom side-effects or integration.
- **Error Handling:** Use `ProvisioningPipelineOptions` to control error handling, step event logging, and hook behavior.
- **Context Injection:** Use `ProvisioningContext` to pass model and services.

---

## 🧩 Creating Custom Steps, Hooks, and Middleware

Your provisioning pipeline is fully extensible.  
Here’s how to build your own steps, hooks, and middleware components:

---

### 1. Custom Steps

A **Step** performs a unit of work—like creating a resource or sending an email.  
You create a custom step by implementing the `ProvisioningStepBase` class.

#### Example:

```csharp
public class SendWelcomeEmailStep : ProvisioningStepBase
{
    public override async Task<T> Execute<T>(ProvisioningContext<T> context)
    {
        // Cast model to your type, then perform your logic
        var user = context.Model as UserModel;
        // ...send email logic here...
        Console.WriteLine($"Welcome email sent to {user.Email}");
    }

    public override async Task<bool> IsExecuted<T>(ProvisioningContext<T> context) {
        return await Task.FromResult(false);
    }
}
```

---

### 2. Custom Hooks

**Hooks** allow you to inject logic before or after pipeline steps (for notifications, auditing, metrics, etc).  
Implement the `IProvisioningHook<T>` interface for type-safe hooks.

#### Example:

```csharp
public class NotificationHook : IProvisioningHook<UserModel>
{
    public Task OnStepStartedAsync(IProvisioningStep step, ProvisioningContext<UserModel> context)
    {
        Console.WriteLine($"Step started: {step.Name}");
        return Task.CompletedTask;
    }

    public Task OnStepCompletedAsync(IProvisioningStep step, ProvisioningContext<UserModel> context)
    {
        Console.WriteLine($"Step completed: {step.Name}");
        return Task.CompletedTask;
    }
}
```

---

### 3. Custom Middleware

**Middleware** wraps pipeline execution, letting you add cross-cutting features like logging, timing, error handling, or retries.  
Implement the `IProvisioningMiddleware<T>` interface.

#### Example:

```csharp
public class TimingMiddleware<T> : IProvisioningMiddleware<T>
{
    public async Task<ProvisioningStepResult> InvokeAsync(IProvisioningStep step, ProvisioningContext<T> context, Func<Task<ProvisioningStepResult>> next)
    {
        var sw = Stopwatch.StartNew();
        await next();
        sw.Stop();
        Console.WriteLine($"Pipeline executed in {sw.ElapsedMilliseconds} ms");
    }
}
```
