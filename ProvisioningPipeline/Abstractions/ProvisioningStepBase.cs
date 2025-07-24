using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;

namespace ProvisioningPipeline.Abstractions {
	public abstract class ProvisioningStepBase : IProvisioningStep {
		public virtual string Name => GetType().Name;
		public abstract Task<T> Execute<T>(ProvisioningContext<T> context) where T : class, new();
		public abstract Task<bool> IsExecuted<T>(ProvisioningContext<T> context) where T : class, new();
	}
}