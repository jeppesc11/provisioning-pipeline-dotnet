﻿using Microsoft.Extensions.Logging;
using ProvisioningPipeline.Abstractions;
using ProvisioningPipeline.Context;

namespace TestApp.Steps {
	internal class Step5 : ProvisioningStepBase {
		public override async Task<T> Execute<T>(ProvisioningContext<T> context) {
			var logger = context.GetService<ILogger>();

			await Task.Delay(100);
			logger.LogInformation($"Executing {this.Name}");

			return await Task.FromResult(context.Model);
		}

		public override async Task<bool> IsExecuted<T>(ProvisioningContext<T> context) {
			return await Task.FromResult(false);
		}
	}
}
