using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;

namespace TestApp.Hooks {
	public class NotificationHook<T> : IProvisioningHook<T> where T : class {
		public Task OnBeforeStepAsync(IProvisioningStep step, ProvisioningContext<T> context) => Task.CompletedTask;

		public Task OnAfterStepAsync(IProvisioningStep step, ProvisioningContext<T> context, bool success, Exception? ex, TimeSpan duration) {
			if (!success && ex != null) {
				Console.WriteLine($"[HOOK][NOTIFICATION] Step {step.Name} failed: {ex.Message}");
			}

			TestProvisioningModel model = context.Model as TestProvisioningModel;

			if (model != null && model.Name == "Jeppe") {
				Console.WriteLine($"[HOOK][NOTIFICATION] Model name is Jeppe");
			}

			return Task.CompletedTask;
		}
	}
}
