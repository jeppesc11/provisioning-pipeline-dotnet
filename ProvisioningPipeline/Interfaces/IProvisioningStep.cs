using ProvisioningPipeline.Context;

namespace ProvisioningPipeline.Interfaces {
	public interface IProvisioningStep {
		string Name { get; }
		Task<T> Execute<T>(ProvisioningContext<T> context) where T : class, new();
		Task<bool> IsExecuted<T>(ProvisioningContext<T> context) where T : class, new();
	}
}