namespace ProvisioningPipeline.Context {
	public class ProvisioningContext<T> where T : class {
		public T Model { get; }

		private readonly Dictionary<Type, object> _services = new();

		public ProvisioningContext(T model) {
			Model = model ?? throw new ArgumentNullException(nameof(model));
		}

		public ProvisioningContext<T> AddService<TService>(TService service) where TService : class {
			_services[typeof(TService)] = service ?? throw new ArgumentNullException(nameof(service));
			return this;
		}

		public TService GetService<TService>() where TService : class {
			if (_services.TryGetValue(typeof(TService), out var service)) {
				return (TService)service;
			}

			throw new InvalidOperationException($"Service of type {typeof(TService).Name} not found.");
		}
	}
}