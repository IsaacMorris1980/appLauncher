using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; // Needed for ConstructorInfo

namespace appLauncher.Core.Services
{
    /// <summary>
    /// A simple, manual Service Locator for dependency injection.
    /// This is a basic implementation to provide compatibility with older UWP SDKs.
    /// </summary>
    public class ServiceLocator
    {
        private readonly Dictionary<Type, Type> _transientRegistrations = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _singletonInstances = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<ServiceLocator, object>> _factoryRegistrations = new Dictionary<Type, Func<ServiceLocator, object>>();

        /// <summary>
        /// Registers a type as a transient service. A new instance will be created each time it's resolved.
        /// </summary>
        /// <typeparam name="TService">The interface or base type of the service.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type of the service.</typeparam>
        public void RegisterTransient<TService, TImplementation>() where TImplementation : TService
        {
            _transientRegistrations[typeof(TService)] = typeof(TImplementation);
        }

        /// <summary>
        /// Registers a type as a singleton service. The same instance will be returned each time it's resolved.
        /// </summary>
        /// <typeparam name="TService">The interface or base type of the service.</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type of the service.</typeparam>
        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            _transientRegistrations.Remove(typeof(TService)); // Ensure it's not also registered as transient
            _singletonInstances[typeof(TService)] = null; // Mark for lazy instantiation
            _transientRegistrations[typeof(TService)] = typeof(TImplementation); // Store implementation type
        }

        /// <summary>
        /// Registers a service with a custom factory function.
        /// </summary>
        /// <typeparam name="TService">The interface or base type of the service.</typeparam>
        /// <param name="factory">A function that creates an instance of the service.</param>
        public void RegisterFactory<TService>(Func<ServiceLocator, TService> factory)
        {
            _factoryRegistrations[typeof(TService)] = locator => factory(locator);
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>An instance of the service.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service cannot be resolved.</exception>
        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <param name="serviceType">The type of the service to resolve.</param>
        /// <returns>An instance of the service.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service cannot be resolved.</exception>
        private object Resolve(Type serviceType)
        {
            // 1. Check for factory registration
            if (_factoryRegistrations.ContainsKey(serviceType))
            {
                return _factoryRegistrations[serviceType](this);
            }

            // 2. Check for singleton instance
            if (_singletonInstances.ContainsKey(serviceType))
            {
                if (_singletonInstances[serviceType] == null)
                {
                    // Lazy instantiate singleton
                    Type implementationType = _transientRegistrations[serviceType]; // Get the implementation type
                    _singletonInstances[serviceType] = CreateInstance(implementationType);
                }
                return _singletonInstances[serviceType];
            }

            // 3. Check for transient registration
            if (_transientRegistrations.ContainsKey(serviceType))
            {
                Type implementationType = _transientRegistrations[serviceType];
                return CreateInstance(implementationType);
            }

            // 4. If serviceType is a concrete type and not registered, try to create it directly
            //    (This mimics AddTransient<T> where T is both service and implementation)
            if (!serviceType.GetTypeInfo().IsInterface && !serviceType.GetTypeInfo().IsAbstract)
            {
                return CreateInstance(serviceType);
            }

            throw new InvalidOperationException($"Service of type {serviceType.FullName} is not registered.");
        }

        /// <summary>
        /// Creates an instance of the given type, resolving its constructor dependencies recursively.
        /// </summary>
        /// <param name="implementationType">The concrete type to instantiate.</param>
        /// <returns>An instance of the type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a suitable constructor cannot be found or dependencies cannot be resolved.</exception>
        private object CreateInstance(Type implementationType)
        {
            // Get the public constructors
            ConstructorInfo[] constructors = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            // Try to find a constructor that we can satisfy all parameters for
            foreach (var constructor in constructors.OrderByDescending(c => c.GetParameters().Length)) // Prefer constructors with more parameters
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                object[] resolvedParameters = new object[parameters.Length];
                bool allParametersResolved = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        resolvedParameters[i] = Resolve(parameters[i].ParameterType);
                    }
                    catch (InvalidOperationException) // If a dependency cannot be resolved
                    {
                        allParametersResolved = false;
                        break;
                    }
                }

                if (allParametersResolved)
                {
                    return constructor.Invoke(resolvedParameters);
                }
            }

            throw new InvalidOperationException($"Could not find a suitable constructor for type {implementationType.FullName} or resolve all its dependencies.");
        }
    }
}
