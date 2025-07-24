using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Models;

namespace ProvisioningPipeline.Middleware.Interfaces {
	public interface IProvisioningMiddleware<T> where T : class {
		Task<ProvisioningStepResult> InvokeAsync(
			IProvisioningStep step,
			ProvisioningContext<T> context,
			Func<Task<ProvisioningStepResult>> next);
	}
}