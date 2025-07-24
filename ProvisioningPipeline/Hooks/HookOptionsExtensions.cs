using ProvisioningPipeline.Interfaces;

namespace ProvisioningPipeline.Hooks {
	public static class HookOptionsExtensions {
		public static void AddForStep<TStep, T>(this List<ProvisioningPipeline.Interfaces.IProvisioningHook<T>> hooks, IProvisioningHook<T> hook)
			where T : class
			where TStep : IProvisioningStep {
			hooks.Add(new FilteredHook<T>(step => step is TStep, hook));
		}

		public static void AddWhen<T>(this List<ProvisioningPipeline.Interfaces.IProvisioningHook<T>> hooks, Func<IProvisioningStep, bool> predicate, IProvisioningHook<T> hook)
			where T : class {
			hooks.Add(new FilteredHook<T>(predicate, hook));
		}
	}
}