using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;

namespace ProvisioningPipeline.Hooks {
	public class FilteredHook<T> : IProvisioningHook<T> where T : class {
		private readonly Func<IProvisioningStep, bool> _predicate;
		private readonly IProvisioningHook<T> _inner;

		public FilteredHook(Func<IProvisioningStep, bool> predicate, IProvisioningHook<T> inner) {
			_predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
			_inner = inner ?? throw new ArgumentNullException(nameof(inner));
		}

		public async Task OnBeforeStepAsync(IProvisioningStep step, ProvisioningContext<T> context) {
			if (_predicate(step)) {
				await _inner.OnBeforeStepAsync(step, context);
			}
		}

		public async Task OnAfterStepAsync(IProvisioningStep step, ProvisioningContext<T> context, bool success, Exception? ex, TimeSpan duration) {
			if (_predicate(step)) {
				await _inner.OnAfterStepAsync(step, context, success, ex, duration);
			}
		}
	}
}