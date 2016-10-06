using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    internal class AttributeActionDiscover : IHandlerActionDiscover
    {
        /// <summary>
        /// Discovers handler actions for the specified type.
        /// </summary>
        /// <param name="type">The type to discover event handler actions.</param>
        /// <returns>The discovered event handler actions.</returns>
        public IEnumerable<HandlerActionDescriptor> Discover(Type type)
        {
#if NetCore
            var methods = type.GetTypeInfo().DeclaredMethods;
#else
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#endif
            foreach (var method in methods)
            {
                var attribute = GetAttribute(method);
                if (attribute != null)
                {
                    yield return new HandlerActionDescriptor
                    {
                        Invoker = HandlerActionInvoker.Create(method, type),
                        Priority = attribute.Priority,
#if !NetCore
                        TransactionFlow = attribute.TransactionFlow
#endif
                    };
                }
            }
        }

        /// <summary>
        /// Discovers handler actions for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to discover event handler actions.</param>
        /// <returns>The discovered event handler actions.</returns>
        public IEnumerable<HandlerActionDescriptor> Discover(object instance)
        {
#if NetCore
            var methods = instance.GetType().GetTypeInfo().DeclaredMethods.Where(method => !method.IsStatic);
#else
            var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#endif
            foreach (var method in methods)
            {
                var attribute = GetAttribute(method);
                if (attribute != null)
                {
                    yield return new HandlerActionDescriptor
                    {
                        Invoker = HandlerActionInvoker.Create(method, instance),
                        Priority = attribute.Priority,
#if !NetCore
                        TransactionFlow = attribute.TransactionFlow
#endif
                    };
                }
            }
        }

        private EventHandlerAttribute GetAttribute(MethodInfo method)
        {
#if Net35
            return (EventHandlerAttribute) Attribute.GetCustomAttribute(method, typeof(EventHandlerAttribute));
#else
            return method.GetCustomAttribute<EventHandlerAttribute>();
#endif
        }
    }
}
