namespace ProvisioningPipeline.Models {
	public class ProvisioningPipelineResult {
		public List<ProvisioningStepResult> Steps { get; } = new();

		public bool Success => Steps.All(step => step.Succeeded);
	}
}