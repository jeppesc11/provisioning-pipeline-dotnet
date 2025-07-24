using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;

namespace ProvisioningPipeline.Models {
	public class StepRegistration<T> where T : class {
		public IProvisioningStep Step { get; }
		public Func<ProvisioningContext<T>, bool> Condition { get; }
		public int? ParallelGroupId { get; }

		public StepRegistration(IProvisioningStep step, Func<ProvisioningContext<T>, bool> condition, int? parallelGroupId = null) {
			Step = step;
			Condition = condition;
			ParallelGroupId = parallelGroupId;
		}
	}
}