using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Middleware.Interfaces;
using ProvisioningPipeline.Models;
using System.Diagnostics;

namespace TestApp.Middleware {
	public class TimingMiddleware<T> : IProvisioningMiddleware<T> where T : class {
		public async Task<ProvisioningStepResult> InvokeAsync(IProvisioningStep step, ProvisioningContext<T> context, Func<Task<ProvisioningStepResult>> next) {
			var stopwatch = Stopwatch.StartNew();
			var result = await next();
			stopwatch.Stop();

			Console.WriteLine($"[TIMER] Step {step.Name} took {stopwatch.ElapsedMilliseconds} ms");
			result.Duration = stopwatch.Elapsed;
			return result;
		}
	}
}