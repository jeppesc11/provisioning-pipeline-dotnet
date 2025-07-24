using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Middleware.Interfaces;
using ProvisioningPipeline.Models;

namespace TestApp.Middleware {
	public class LoggingMiddleware<T> : IProvisioningMiddleware<T> where T : class {
		public async Task<ProvisioningStepResult> InvokeAsync(IProvisioningStep step, ProvisioningContext<T> context, Func<Task<ProvisioningStepResult>> next) {
			Console.WriteLine($"[LOG] Starting step: {step.Name}");
			var result = await next();
			Console.WriteLine($"[LOG] Completed step: {step.Name} (Success: {result.Succeeded})");
			return result;
		}
	}
}