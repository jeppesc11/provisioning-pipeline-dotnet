using Microsoft.Extensions.Logging;
using ProvisioningPipeline.Abstractions;
using ProvisioningPipeline.Context;

namespace TestApp.Steps {
	internal class Step2 : ProvisioningStepBase {

		public override async Task<T> Execute<T>(ProvisioningContext<T> context) {
			var logger = context.GetService<ILogger>();

			logger.LogInformation($"Executing {this.Name}");

			return await Task.FromResult(context.Model);
		}

		public override async Task<bool> IsExecuted<T>(ProvisioningContext<T> context) {
			return await Task.FromResult(false);
		}
	}
}
