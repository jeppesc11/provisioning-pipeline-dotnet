using ProvisioningPipeline.Interfaces;

namespace ProvisioningPipeline.Models {
	public class ProvisioningPipelineOptions<T> where T : class {
		internal List<(Predicate<IProvisioningStep> Filter, IProvisioningHook<T> Hook)> HookRegistrations { get; } = new();

		public Action<IProvisioningStep>? OnStepStart { get; set; }
		public Action<IProvisioningStep>? OnStepComplete { get; set; }
		public Action<IProvisioningStep, Exception>? OnError { get; set; }
		public bool ContinueOnError { get; set; }
		public List<IProvisioningHook<T>> Hooks { get; } = new();

		public void AddHookFor(Func<IProvisioningStep, bool> predicate, IProvisioningHook<T> hook) {
			HookRegistrations.Add((new Predicate<IProvisioningStep>(predicate), hook));
		}
	}
}