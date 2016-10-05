using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBuster.UnitTests
{
    public class EventBusFixture
    {
        private class CustomHandlerActionDiscover : IHandlerActionDiscover
        {
            public IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, Type type)
            {
                var methods = type.GetMethods();
                return from method in methods
                    where method.Name == "CustomAction"
                    select new HandlerActionDescriptor()
                    {
                        Invoker = new ReflectionActionInvoker(method, type)
                    };
            }

            public IEnumerable<HandlerActionDescriptor> Discover(IServiceProvider serviceProvider, object instance)
            {
                var type = instance.GetType();
                var methods = type.GetMethods();
                return from method in methods
                       where method.Name == "CustomAction"
                       select new HandlerActionDescriptor()
                       {
                           Invoker = new ReflectionActionInvoker(method, type)
                       };
            }
        }

        private class CustomHandlerActivator : IHandlerActivator
        {
            public object Create(IServiceProvider serviceProvider, Type handlerType)
            {
                var instance = Activator.CreateInstance(handlerType);
                var property = handlerType.GetProperty("CustomState");
                property?.SetValue(instance, "EventBus", new object[0]);
                return instance;
            }

            public void Release(object handlerInstance)
            {
                (handlerInstance as IDisposable)?.Dispose();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void RegisterCustomDiscover()
        {
            var discover = new CustomHandlerActionDiscover();
            EventBus.Default.Discovers.Add(discover);
            EventBus.Register<CustomEventTarget>();

            try
            {
                CustomEventTarget.GlobalState = null;
                EventBus.Trigger(new UpdateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", CustomEventTarget.GlobalState);
            }
            finally
            {
                EventBus.Unregister<CustomEventTarget>();
                EventBus.Default.Discovers.Remove(discover);
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void RegisterCustomActivator()
        {
            EventBus.Default.SetServiceProvider(() =>
            {
                var serviceProvider = new ServiceProvider();
                serviceProvider.AddInstance<IHandlerActivator>(new CustomHandlerActivator());
                return serviceProvider;
            });
            EventBus.Register<CustomActivateTarget>();

            try
            {
                CustomActivateTarget.StaticState = null;
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBus", CustomActivateTarget.StaticState);
            }
            finally
            {
                EventBus.Unregister<CustomEventTarget>();
                ((DefaultEventBus)EventBus.Default).ResetServiceProvider();
            }
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void RegisterServiceBasedActivator()
        {
            var targetInstance = new CustomActivateTarget {InstanceState = "EventBus" };
            EventBus.Default.SetServiceProvider(() =>
            {
                var serviceProvider = new ServiceProvider();
                serviceProvider.AddInstance<IHandlerActivator>(new ServiceBasedHandlerActivator());
                serviceProvider.AddInstance<CustomActivateTarget>(targetInstance);
                return serviceProvider;
            });
            EventBus.Register<CustomActivateTarget>();
            try
            {
                CustomActivateTarget.StaticState = null;
                EventBus.Trigger(new CreateUserEvent { UserName = "EventBuster" });
                Assert.Equal("EventBuster", targetInstance.InstanceState);
            }
            finally
            {
                EventBus.Unregister<CustomEventTarget>();
                ((DefaultEventBus)EventBus.Default).ResetServiceProvider();
            }
        }
    }
}
