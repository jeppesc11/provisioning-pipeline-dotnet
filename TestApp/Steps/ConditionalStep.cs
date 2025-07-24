using Microsoft.Extensions.Logging;
using ProvisioningPipeline.Abstractions;
using ProvisioningPipeline.Context;

namespace TestApp.Steps {
	internal class ConditionalStep : ProvisioningStepBase {
		public override Task<T> Execute<T>(ProvisioningContext<T> context) {
			var logger = context.GetService<ILogger>();
			logger.LogInformation("Executing Conditional Step: Age > 18");

			return Task.FromResult(context.Model);
		}

		public override Task<bool> IsExecuted<T>(ProvisioningContext<T> context) {
			return Task.FromResult(false);
		}
	}
}