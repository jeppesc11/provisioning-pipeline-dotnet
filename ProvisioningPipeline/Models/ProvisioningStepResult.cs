namespace ProvisioningPipeline.Models {
	public class ProvisioningStepResult {
		public string StepName { get; set; } = string.Empty;
		public bool Executed { get; set; }
		public bool Succeeded { get; set; }
		public TimeSpan Duration { get; set; }
		public Exception? Error { get; set; }
	}
}