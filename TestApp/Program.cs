using Microsoft.Extensions.Logging;
using ProvisioningPipeline;
using ProvisioningPipeline.Context;
using ProvisioningPipeline.Hooks;
using TestApp.Hooks;
using TestApp.Middleware;
using TestApp.Steps;

namespace TestApp {
	internal class Program {
		async static Task Main(string[] args) {

			var model = new TestProvisioningModel { Name = "Dan", Age = 28, Email = "jeppe@example.com" };

			var context = new ProvisioningContext<TestProvisioningModel>(model)
				.AddService<ILogger>(new ConsoleLogger());

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

			ProvisioningPipeline.Models.ProvisioningPipelineResult result = await pipeline.ExecuteAsync(context);
		}
	}
}