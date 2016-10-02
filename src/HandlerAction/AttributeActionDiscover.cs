using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

        private static void ValidateMethod(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition || method.IsGenericMethod)
            {
                ThrowHelper.ThrowGenericHandlerActionMethodException(method);
            }
            if ((typeof(Task).IsAssignableFrom(method.ReturnType) && method.ReturnType != typeof(Task)) || method.ReturnType != typeof(void))
            {
                ThrowHelper.ThrowHandlerActionMethodCannotReturnException(method);
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
            if (method.ReturnType == typeof(Task))
            {
                if (parameters.Length == 0 || parameters.Length > 2)
                {
                    ThrowHelper.ThrowInvalidAsyncHandlerActionMethodException(method);
                }
                if (parameters.Length >= 1 && parameters[0].ParameterType == typeof(CancellationToken))
                {
                    ThrowHelper.ThrowAsyncHandlerActionMethodInvalidFirstParameter(method);
                }
                if (parameters.Length == 2 && parameters[1].ParameterType != typeof(CancellationToken))
                {
                    ThrowHelper.ThrowAsyncHandlerActionMethodInvalidSecondParameter(method);
                }
            }
        }

        private static IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, Type type, Func<MethodInfo, IHandlerActionInvoker> invokerCreator)
        {
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attribute = method.GetCustomAttribute<EventHandlerAttribute>();
                if (attribute != null)
                {
                    ValidateMethod(method);
                    yield return new HandlerActionDescriptor
                    {
                        Invoker = invokerCreator(method),
                        Priority = attribute.Priority,
                        TransactionFlow = attribute.TransactionFlow,
                        Services = serviceProvider
                    };
                }
            }
        }
    }
}
