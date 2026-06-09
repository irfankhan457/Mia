using System;
using System.Collections.Generic;

namespace MiaFantasyRun.Core
{
    public sealed class ServiceRegistry
    {
        private readonly Dictionary<Type, object> services = new();

        public void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
        }

        public T Resolve<T>() where T : class
        {
            if (services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Service not registered: {typeof(T).Name}");
        }

        public bool TryResolve<T>(out T service) where T : class
        {
            if (services.TryGetValue(typeof(T), out var value))
            {
                service = (T)value;
                return true;
            }

            service = null;
            return false;
        }
    }
}
