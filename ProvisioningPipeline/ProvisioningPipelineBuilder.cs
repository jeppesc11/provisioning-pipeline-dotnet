using ProvisioningPipeline.Context;
using ProvisioningPipeline.Hooks;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Middleware;
using ProvisioningPipeline.Middleware.Interfaces;
using ProvisioningPipeline.Models;
using ProvisioningPipeline.Pipeline;

namespace ProvisioningPipeline {
	public class ProvisioningPipelineBuilder<T> where T : class, new() {
		private readonly List<StepRegistration<T>> _registrations = new();
		private readonly MiddlewarePipeline<T> _middlewarePipeline = new();
		private int _parallelGroupCounter = 0;

		public ProvisioningPipelineBuilder<T> AddStep(IProvisioningStep step) {
			return AddStepWhen(_ => true, step);
		}

		public ProvisioningPipelineBuilder<T> AddStepWhen(Func<ProvisioningContext<T>, bool> condition, IProvisioningStep step) {
			_registrations.Add(new StepRegistration<T>(step, condition));
			return this;
		}

		public ProvisioningPipelineBuilder<T> AddParallelSteps(params IProvisioningStep[] steps) {
			int groupId = ++_parallelGroupCounter;
			foreach (var step in steps) {
				_registrations.Add(new StepRegistration<T>(step, _ => true, groupId));
			}
			return this;
		}

		public ProvisioningPipelineBuilder<T> UseMiddleware(IProvisioningMiddleware<T> middleware) {
			_middlewarePipeline.Use(middleware);
			return this;
		}

		public ProvisioningPipeline<T> Build(Action<ProvisioningPipelineOptions<T>>? configure = null) {
			var options = new ProvisioningPipelineOptions<T>();
			configure?.Invoke(options);

			foreach (var (filter, hook) in options.HookRegistrations) {
				foreach (var step in _registrations.Select(r => r.Step).Distinct()) {
					if (filter(step)) {
						options.Hooks.Add(new FilteredHook<T>(s => s == step, hook));
					}
				}
			}

			return new ProvisioningPipeline<T>(
				_registrations,
				options.Hooks,
				options.OnStepStart,
				options.OnStepComplete,
				options.OnError,
				options.ContinueOnError,
				_middlewarePipeline
			);
		}
	}
}