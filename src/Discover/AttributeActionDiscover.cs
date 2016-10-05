using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    internal class AttributeActionDiscover : IHandlerActionDiscover
    {
        public IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, Type type)
        {
            return Discover(serviceProvider, type, method => new ReflectionActionInvoker(method, type));
        }

        public IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, object instance)
        {
            return Discover(serviceProvider, instance.GetType(), method => new ReflectionActionInvoker(method, instance));
        }
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
            if(method.ReturnType != typeof(void))
            {
                ThrowHelper.ThrowHandlerActionMethodCannotReturnException(method);
            }
#endif
        }

        private static IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, Type type, Func<MethodInfo, IHandlerActionInvoker> invokerCreator)
        {
#if NetCore
            var methods = type.GetTypeInfo().DeclaredMethods;
#else
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#endif
            foreach (var method in methods)
            {
#if Net35
                var attribute = (EventHandlerAttribute) Attribute.GetCustomAttribute(method, typeof(EventHandlerAttribute));
#else
                var attribute = method.GetCustomAttribute<EventHandlerAttribute>();
#endif
                if (attribute != null)
                {
                    ValidateMethod(method);
                    yield return new HandlerActionDescriptor
                    {
                        Invoker = invokerCreator(method),
                        Priority = attribute.Priority,
#if !NetCore
                        TransactionFlow = attribute.TransactionFlow
#endif
                    };
                }
            }
        }
    }
}
