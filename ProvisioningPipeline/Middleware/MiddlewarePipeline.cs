using ProvisioningPipeline.Context;
using ProvisioningPipeline.Interfaces;
using ProvisioningPipeline.Middleware.Interfaces;
using ProvisioningPipeline.Models;

namespace ProvisioningPipeline.Middleware {
	public class MiddlewarePipeline<T> where T : class {
		private readonly List<IProvisioningMiddleware<T>> _middlewares = new();

		public MiddlewarePipeline<T> Use(IProvisioningMiddleware<T> middleware) {
			_middlewares.Add(middleware);
			return this;
		}

		public Func<Task<ProvisioningStepResult>> Build(
			IProvisioningStep step,
			ProvisioningContext<T> context,
			Func<Task<ProvisioningStepResult>> finalHandler) {
			Func<Task<ProvisioningStepResult>> pipeline = finalHandler;

			foreach (var middleware in _middlewares.AsEnumerable().Reverse()) {
				var next = pipeline;
				pipeline = () => middleware.InvokeAsync(step, context, next);
			}

			return pipeline;
		}

		public IReadOnlyList<IProvisioningMiddleware<T>> Middlewares => _middlewares;
	}
}