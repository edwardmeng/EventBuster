using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventBuster
{
    internal delegate object ObjectFactory(IServiceProvider serviceProvider, object[] arguments);
    /// <summary>
    /// Helper code for the various activator services.
    /// </summary>
    internal static class ActivatorUtilities
    {
        private static readonly MethodInfo GetServiceInfo =
           GetMethodInfo<Func<IServiceProvider, Type, Type, bool, object>>((sp, t, r, c) => GetService(sp, t, r, c));

        private static object GetService(IServiceProvider sp, Type type, Type requiredBy, bool isDefaultParameterRequired)
        {
            var service = sp.GetService(type);
            if (service == null && !isDefaultParameterRequired)
            {
                ThrowHelper.ThrowActivatorCannotResolveParameterException(type, requiredBy);
            }
            return service;
        }

        private static MethodInfo GetMethodInfo<T>(Expression<T> expr)
        {
            var mc = (MethodCallExpression)expr.Body;
            return mc.Method;
        }

        private static Expression BuildFactoryExpression(ConstructorInfo constructor, int?[] parameterMap, Expression serviceProvider, Expression factoryArgumentArray)
        {
            var constructorParameters = constructor.GetParameters();
            var constructorArguments = new Expression[constructorParameters.Length];

            for (var i = 0; i < constructorParameters.Length; i++)
            {
                var parameterType = constructorParameters[i].ParameterType;

                if (parameterMap[i] != null)
                {
#if Net35
                    constructorArguments[i] = Expression.ArrayIndex(factoryArgumentArray, Expression.Constant(parameterMap[i]));
#else
                    constructorArguments[i] = Expression.ArrayAccess(factoryArgumentArray, Expression.Constant(parameterMap[i]));
#endif
                }
                else
                {
#if Net35
                    var constructorParameterHasDefault = false;
#else
                    var constructorParameterHasDefault = constructorParameters[i].HasDefaultValue;
#endif
                    var parameterTypeExpression = new[] { serviceProvider,
                        Expression.Constant(parameterType, typeof(Type)),
                        Expression.Constant(constructor.DeclaringType, typeof(Type)),
                        Expression.Constant(constructorParameterHasDefault) };
                    constructorArguments[i] = Expression.Call(GetServiceInfo, parameterTypeExpression);
                }
#if !Net35
                // Support optional constructor arguments by passing in the default value
                // when the argument would otherwise be null.
                if (constructorParameters[i].HasDefaultValue)
                {
                    var defaultValueExpression = Expression.Constant(constructorParameters[i].DefaultValue);
                    constructorArguments[i] = Expression.Coalesce(constructorArguments[i], defaultValueExpression);
                }
#endif

                constructorArguments[i] = Expression.Convert(constructorArguments[i], parameterType);
            }

            return Expression.New(constructor, constructorArguments);
        }

        private static void FindApplicableConstructor(Type instanceType, Type[] argumentTypes, out ConstructorInfo matchingConstructor, out int?[] parameterMap)
        {
            matchingConstructor = null;
            parameterMap = null;
#if NetCore
            var constructors = instanceType.GetTypeInfo().DeclaredConstructors.Where(ctor => !ctor.IsStatic && ctor.IsPublic);
#else
            var constructors = instanceType.GetConstructors();
#endif
            foreach (var constructor in constructors)
            {
                int?[] tempParameterMap;
                if (TryCreateParameterMap(constructor.GetParameters(), argumentTypes, out tempParameterMap))
                {
                    if (matchingConstructor != null)
                    {
                        ThrowHelper.ThrowActivatorAmbiguousConstructorsException(instanceType);
                    }

                    matchingConstructor = constructor;
                    parameterMap = tempParameterMap;
                }
            }

            if (matchingConstructor == null)
            {
                ThrowHelper.ThrowActivatorCannotLocateConstructorException(instanceType);
            }
        }

        // Creates an injective parameterMap from givenParameterTypes to assignable constructorParameters.
        // Returns true if each given parameter type is assignable to a unique; otherwise, false.
        private static bool TryCreateParameterMap(ParameterInfo[] constructorParameters, Type[] argumentTypes, out int?[] parameterMap)
        {
            parameterMap = new int?[constructorParameters.Length];

            for (var i = 0; i < argumentTypes.Length; i++)
            {
                var foundMatch = false;
#if NetCore
                var givenParameter = argumentTypes[i].GetTypeInfo();
#else
                var givenParameter = argumentTypes[i];
#endif

                for (var j = 0; j < constructorParameters.Length; j++)
                {
                    if (parameterMap[j] != null)
                    {
                        // This ctor parameter has already been matched
                        continue;
                    }
#if NetCore
                    var matchParameter = constructorParameters[j].ParameterType.GetTypeInfo().IsAssignableFrom(givenParameter);
#else
                    var matchParameter = constructorParameters[j].ParameterType.IsAssignableFrom(givenParameter);
#endif
                    if (matchParameter)
                    {
                        foundMatch = true;
                        parameterMap[j] = i;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="provider">The service provider used to resolve dependencies</param>
        /// <param name="instanceType">The type to activate</param>
        /// <param name="parameters">Constructor arguments not provided by the <paramref name="provider"/>.</param>
        /// <returns>An activated object of type instanceType</returns>
        public static object CreateInstance(IServiceProvider provider, Type instanceType, params object[] parameters)
        {
            int bestLength = -1;
            ConstructorMatcher bestMatcher = null;
#if NetCore
            var contructorMatchers = instanceType.GetTypeInfo().DeclaredConstructors
                .Where(c => !c.IsStatic && c.IsPublic)
                .Select(constructor => new ConstructorMatcher(constructor));
#else
            var contructorMatchers = instanceType.GetConstructors().Select(constructor => new ConstructorMatcher(constructor));
#endif
            foreach (var matcher in contructorMatchers)
            {
                var length = matcher.Match(parameters);
                if (length == -1)
                {
                    continue;
                }
                if (bestLength < length)
                {
                    bestLength = length;
                    bestMatcher = matcher;
                }
            }

            if (bestMatcher == null)
            {
                ThrowHelper.ThrowActivatorCannotLocateConstructorException(instanceType);
            }

            return bestMatcher.CreateInstance(provider);
        }

        /// <summary>
        /// Create a delegate that will instantiate a type with constructor arguments provided directly
        /// and/or from an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="instanceType">The type to activate</param>
        /// <param name="argumentTypes">
        /// The types of objects, in order, that will be passed to the returned function as its second parameter
        /// </param>
        /// <returns>
        /// A factory that will instantiate instanceType using an <see cref="IServiceProvider"/>
        /// and an argument array containing objects matching the types defined in argumentTypes
        /// </returns>
        public static ObjectFactory CreateFactory(Type instanceType, Type[] argumentTypes)
        {
            ConstructorInfo constructor;
            int?[] parameterMap;

            FindApplicableConstructor(instanceType, argumentTypes, out constructor, out parameterMap);

            var provider = Expression.Parameter(typeof(IServiceProvider), "provider");
            var argumentArray = Expression.Parameter(typeof(object[]), "argumentArray");
            var factoryExpressionBody = BuildFactoryExpression(constructor, parameterMap, provider, argumentArray);

            var factoryLamda = Expression.Lambda<Func<IServiceProvider, object[], object>>(
                factoryExpressionBody, provider, argumentArray);

            var result = factoryLamda.Compile();
            return result.Invoke;
        }

        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type to activate</typeparam>
        /// <param name="provider">The service provider used to resolve dependencies</param>
        /// <param name="parameters">Constructor arguments not provided by the <paramref name="provider"/>.</param>
        /// <returns>An activated object of type T</returns>
        public static T CreateInstance<T>(IServiceProvider provider, params object[] parameters)
        {
            return (T)CreateInstance(provider, typeof(T), parameters);
        }

        /// <summary>
        /// Retrieve an instance of the given type from the service provider. If one is not found then instantiate it directly.
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <param name="provider">The service provider used to resolve dependencies</param>
        /// <returns>The resolved service or created instance</returns>
        public static T GetServiceOrCreateInstance<T>(IServiceProvider provider)
        {
            return (T)GetServiceOrCreateInstance(provider, typeof(T));
        }

        /// <summary>
        /// Retrieve an instance of the given type from the service provider. If one is not found then instantiate it directly.
        /// </summary>
        /// <param name="provider">The service provider</param>
        /// <param name="type">The type of the service</param>
        /// <returns>The resolved service or created instance</returns>
        public static object GetServiceOrCreateInstance(IServiceProvider provider, Type type)
        {
            return provider.GetService(type) ?? CreateInstance(provider, type);
        }

        private class ConstructorMatcher
        {
            private readonly ConstructorInfo _constructor;
            private readonly ParameterInfo[] _parameters;
            private readonly object[] _parameterValues;
            private readonly bool[] _parameterValuesSet;

            public ConstructorMatcher(ConstructorInfo constructor)
            {
                _constructor = constructor;
                _parameters = _constructor.GetParameters();
                _parameterValuesSet = new bool[_parameters.Length];
                _parameterValues = new object[_parameters.Length];
            }

            public int Match(object[] givenParameters)
            {

                var applyIndexStart = 0;
                var applyExactLength = 0;
                for (var givenIndex = 0; givenIndex != givenParameters.Length; givenIndex++)
                {
#if NetCore
                    var givenType = givenParameters[givenIndex] == null ? null : givenParameters[givenIndex].GetType().GetTypeInfo();
#else        
                    var givenType = givenParameters[givenIndex] == null ? null : givenParameters[givenIndex].GetType();
#endif
                    var givenMatched = false;

                    for (var applyIndex = applyIndexStart; givenMatched == false && applyIndex != _parameters.Length; ++applyIndex)
                    {
                        if (_parameterValuesSet[applyIndex] == false &&
#if NetCore
                            _parameters[applyIndex].ParameterType.GetTypeInfo().IsAssignableFrom(givenType)
#else
                            _parameters[applyIndex].ParameterType.IsAssignableFrom(givenType)
#endif
                            )
                        {
                            givenMatched = true;
                            _parameterValuesSet[applyIndex] = true;
                            _parameterValues[applyIndex] = givenParameters[givenIndex];
                            if (applyIndexStart == applyIndex)
                            {
                                applyIndexStart++;
                                if (applyIndex == givenIndex)
                                {
                                    applyExactLength = applyIndex;
                                }
                            }
                        }
                    }

                    if (givenMatched == false)
                    {
                        return -1;
                    }
                }
                return applyExactLength;
            }

            public object CreateInstance(IServiceProvider provider)
            {
                for (var index = 0; index != _parameters.Length; index++)
                {
                    if (_parameterValuesSet[index] == false)
                    {
                        var value = provider.GetService(_parameters[index].ParameterType);
                        if (value == null)
                        {
#if Net35
                            ThrowHelper.ThrowActivatorCannotResolveParameterException(_parameters[index].ParameterType, _constructor.DeclaringType);
#else
                            if (!_parameters[index].HasDefaultValue)
                            {
                                ThrowHelper.ThrowActivatorCannotResolveParameterException(_parameters[index].ParameterType, _constructor.DeclaringType);
                            }
                            else
                            {
                                _parameterValues[index] = _parameters[index].DefaultValue;
                            }
#endif
                        }
                        else
                        {
                            _parameterValues[index] = value;
                        }
                    }
                }

#if !Net35
                try
                {
                    return _constructor.Invoke(_parameterValues);
                }
                catch (Exception ex)
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                    // The above line will always throw, but the compiler requires we throw explicitly.
                    throw;
                }
#else
                return _constructor.Invoke(_parameterValues);
#endif
            }
        }
    }
}
