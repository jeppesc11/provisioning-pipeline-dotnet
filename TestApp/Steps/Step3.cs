using Microsoft.Extensions.Logging;
using ProvisioningPipeline.Abstractions;
using ProvisioningPipeline.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Steps {
	internal class Step3 : ProvisioningStepBase {

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
