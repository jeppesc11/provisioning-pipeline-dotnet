using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Middleware;
using ProvisioningPipeline.Models;
using System.Diagnostics;

namespace ProvisioningPipeline.Pipeline {
	public class ProvisioningPipeline<T> where T : class, new() {
		private readonly List<StepRegistration<T>> _registrations;
		private readonly List<IProvisioningHook<T>> _hooks;
		private readonly Action<IProvisioningStep>? _onStepStart;
		private readonly Action<IProvisioningStep>? _onStepComplete;
		private readonly Action<IProvisioningStep, Exception>? _onError;
		private readonly bool _continueOnError;
		private readonly MiddlewarePipeline<T> _middlewarePipeline;

		public ProvisioningPipeline(
			IEnumerable<StepRegistration<T>> registrations,
			List<IProvisioningHook<T>> hooks,
			Action<IProvisioningStep>? onStepStart,
			Action<IProvisioningStep>? onStepComplete,
			Action<IProvisioningStep, Exception>? onError,
			bool continueOnError,
			MiddlewarePipeline<T>? middlewarePipeline = null) {
			_registrations = registrations.ToList();
			_hooks = hooks;
			_onStepStart = onStepStart;
			_onStepComplete = onStepComplete;
			_onError = onError;
			_continueOnError = continueOnError;
			_middlewarePipeline = middlewarePipeline ?? new MiddlewarePipeline<T>();
		}

		public async Task<ProvisioningPipelineResult> ExecuteAsync(ProvisioningContext<T> context) {
			var result = new ProvisioningPipelineResult();

			int? currentParallelGroup = null;
			var parallelSteps = new List<StepRegistration<T>>();

			foreach (var reg in _registrations) {
				if (!reg.Condition(context)) { continue; }

				if (reg.ParallelGroupId.HasValue) {
					if (currentParallelGroup == null || currentParallelGroup == reg.ParallelGroupId) {
						currentParallelGroup = reg.ParallelGroupId;
						parallelSteps.Add(reg);
						continue;
					}

					var results = await ExecuteParallelGroup(parallelSteps, context);
					result.Steps.AddRange(results);

					parallelSteps = new List<StepRegistration<T>> { reg };
					currentParallelGroup = reg.ParallelGroupId;
					continue;
				}

				if (parallelSteps.Any()) {
					var results = await ExecuteParallelGroup(parallelSteps, context);
					result.Steps.AddRange(results);
					parallelSteps.Clear();
				}

				var stepResult = await RunStepAsync(reg.Step, context);
				result.Steps.Add(stepResult);
			}

			if (parallelSteps.Any()) {
				var results = await ExecuteParallelGroup(parallelSteps, context);
				result.Steps.AddRange(results);
			}

			return result;
		}

		private async Task<List<ProvisioningStepResult>> ExecuteParallelGroup(List<StepRegistration<T>> group, ProvisioningContext<T> context) {
			var tasks = group.Select(reg => RunStepAsync(reg.Step, context));
			var results = await Task.WhenAll(tasks);
			return results.ToList();
		}

		private async Task<ProvisioningStepResult> RunStepAsync(IProvisioningStep step, ProvisioningContext<T> context) {
			_onStepStart?.Invoke(step);
			Func<Task<ProvisioningStepResult>> coreHandler = async () =>
			{
				var stopwatch = Stopwatch.StartNew();
				Exception? error = null;
				var success = false;

				foreach (var hook in _hooks) {
					await hook.OnBeforeStepAsync(step, context);
				}

				try {
					if (!await step.IsExecuted(context)) {
						await step.Execute(context);
					}

					success = true;
				} catch (Exception ex) {
					error = ex;
					_onError?.Invoke(step, ex);
					if (!_continueOnError) throw;
				}

				stopwatch.Stop();

				foreach (var hook in _hooks) {
					await hook.OnAfterStepAsync(step, context, success, error, stopwatch.Elapsed);
				}

				_onStepComplete?.Invoke(step);

				return new ProvisioningStepResult {
					StepName = step.Name,
					Executed = true,
					Succeeded = success,
					Error = error,
					Duration = stopwatch.Elapsed
				};
			};

			var pipeline = _middlewarePipeline.Build(step, context, coreHandler);
			return await pipeline();
		}
	}
}