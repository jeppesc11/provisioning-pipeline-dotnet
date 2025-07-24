using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Middleware.Interfaces;
using ProvisioningPipeline.Models;

namespace TestApp.Middleware {
	public class RetryMiddleware<T> : IProvisioningMiddleware<T> where T : class {
		private readonly int _maxRetries;
		private readonly TimeSpan _delay;

		public RetryMiddleware(int maxRetries = 3, TimeSpan? delay = null) {
			_maxRetries = maxRetries;
			_delay = delay ?? TimeSpan.FromSeconds(1);
		}

		public async Task<ProvisioningStepResult> InvokeAsync(IProvisioningStep step, ProvisioningContext<T> context, Func<Task<ProvisioningStepResult>> next) {
			for (int attempt = 1; attempt <= _maxRetries; attempt++) {
				try {
					return await next();
				} catch (Exception ex) when (attempt < _maxRetries) {
					Console.WriteLine($"[RETRY] Step {step.Name} failed (attempt {attempt}). Retrying in {_delay.TotalSeconds}s...");
					await Task.Delay(_delay);
				}
			}

			// Final attempt
			return await next();
		}
	}
}