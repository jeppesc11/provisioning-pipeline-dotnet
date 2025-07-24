using Microsoft.Extensions.Logging;
using ProvisioningPipeline.Abstractions;
using ProvisioningPipeline.Context;

namespace TestApp.Steps {
	internal class Step1 : ProvisioningStepBase {
		private readonly string? TempMessage;

		public Step1() { }

		public Step1(string tempMessage) {
			TempMessage = tempMessage;
		}

		public override async Task<T> Execute<T>(ProvisioningContext<T> context) {
			var logger = context.GetService<ILogger>();

			if (!string.IsNullOrWhiteSpace(TempMessage)) {
				logger.LogInformation($"Executing {this.Name} - TempMessage: " + TempMessage);
			} else {
				logger.LogInformation($"Executing {this.Name}");
			}

			return await Task.FromResult(context.Model);
		}

		public override async Task<bool> IsExecuted<T>(ProvisioningContext<T> context) {
			return await Task.FromResult(false);
		}
	}
}
