using ProvisioningPipeline.Context;

namespace ProvisioningPipeline.Interfaces {
	public interface IProvisioningHook<T> where T : class {
		Task OnBeforeStepAsync(IProvisioningStep step, ProvisioningContext<T> context);
		Task OnAfterStepAsync(IProvisioningStep step, ProvisioningContext<T> context, bool success, Exception? ex, TimeSpan duration);
	}
}