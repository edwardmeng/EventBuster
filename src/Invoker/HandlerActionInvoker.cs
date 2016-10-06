using System;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    /// <summary>
    /// Providers the convenient methods to create event handler action invokers.
    /// </summary>
    public static class HandlerActionInvoker
    {
#if !Net35
        private static bool IsAsyncMethod(MethodInfo method)
        {
#if NetCore
            return typeof(System.Threading.Tasks.Task).GetTypeInfo().IsAssignableFrom(method.ReturnType.GetTypeInfo());
#else
            return typeof(System.Threading.Tasks.Task).IsAssignableFrom(method.ReturnType);
#endif
        }

#endif
        private static void ValidateMethod(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (method.IsGenericMethodDefinition || method.IsGenericMethod)
            {
                ThrowHelper.ThrowGenericHandlerActionMethodException(method);
            }
            var parameters = method.GetParameters();
            if (parameters.Any(parameter => parameter.IsOut || parameter.ParameterType.IsByRef))
            {
                ThrowHelper.ThrowHandlerActionMethodMustBeInputParameterException(method);
            }
            if (method.ReturnType == typeof(void) && parameters.Length != 1)
            {
                ThrowHelper.ThrowHandlerActionMethodMustBeSingleParameterException(method);
            }
#if !Net35
            var isAsync = IsAsyncMethod(method);
            if ((isAsync && method.ReturnType != typeof(System.Threading.Tasks.Task)) || (!isAsync && method.ReturnType != typeof(void)))
            {
                ThrowHelper.ThrowHandlerActionMethodCannotReturnException(method);
            }
            if (method.ReturnType == typeof(System.Threading.Tasks.Task))
            {
                if (parameters.Length == 0 || parameters.Length > 2)
                {
                    ThrowHelper.ThrowInvalidAsyncHandlerActionMethodException(method);
                }
                if (parameters.Length >= 1 && parameters[0].ParameterType == typeof(System.Threading.CancellationToken))
                {
                    ThrowHelper.ThrowAsyncHandlerActionMethodInvalidFirstParameter(method);
                }
                if (parameters.Length == 2 && parameters[1].ParameterType != typeof(System.Threading.CancellationToken))
                {
                    ThrowHelper.ThrowAsyncHandlerActionMethodInvalidSecondParameter(method);
                }
            }
#else
            if (method.ReturnType != typeof(void))
            {
                ThrowHelper.ThrowHandlerActionMethodCannotReturnException(method);
            }
#endif
        }

        /// <summary>
        /// Creates event handler action invoker by using the specified <see cref="MethodInfo"/> and its reflected type.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> to create event handler action invoker.</param>
        /// <param name="type">The <see cref="Type"/> of reflecting methods.</param>
        /// <returns>The new instance of <see cref="IHandlerActionInvoker"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static IHandlerActionInvoker Create(MethodInfo method, Type type = null)
        {
            ValidateMethod(method);
            return new ReflectionActionInvoker(method, type);
        }

        /// <summary>
        /// Creates event handler action invoker by using the specified <see cref="MethodInfo"/> and 
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> to create event handler action invoker.</param>
        /// <param name="instance">The instance to invoke handler action method.</param>
        /// <returns>The new instance of <see cref="IHandlerActionInvoker"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static IHandlerActionInvoker Create(MethodInfo method, object instance = null)
        {
            ValidateMethod(method);
            if (method.IsStatic)
            {
                ThrowHelper.ThrowHandlerActionMethodCannotStaticException(method);
            }
            return new ReflectionActionInvoker(method, instance);
        }

        /// <summary>
        /// Creates event handler action invoker by using the specified lambda action.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="action">The lambda action to handle event.</param>
        /// <returns>The new instance of <see cref="IHandlerActionInvoker"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public static IHandlerActionInvoker Create<TEvent>(Action<TEvent> action)
        {
            return new LambdaActionInvoker<TEvent>(action);
        }

#if !Net35

        /// <summary>
        /// Creates event handler action invoker by using the specified lambda action.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The lambda action to handle event.</param>
        /// <returns>The new instance of <see cref="IHandlerActionInvoker"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static IHandlerActionInvoker Create<TEvent>(Func<TEvent, System.Threading.Tasks.Task> asyncAction)
        {
            return new LambdaActionInvoker<TEvent>(asyncAction);
        }

        /// <summary>
        /// Creates event handler action invoker by using the specified lambda action.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="asyncAction">The lambda action to handle event.</param>
        /// <returns>The new instance of <see cref="IHandlerActionInvoker"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="asyncAction"/> is null.</exception>
        public static IHandlerActionInvoker Create<TEvent>(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> asyncAction)
        {
            return new LambdaActionInvoker<TEvent>(asyncAction);
        }

#endif
    }
}
