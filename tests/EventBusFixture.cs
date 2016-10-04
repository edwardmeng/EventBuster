using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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
                property?.SetValue(instance, "EventBus");
                return instance;
            }

            public void Release(object handlerInstance)
            {
                (handlerInstance as IDisposable)?.Dispose();
            }
        }

        [Test]
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

        [Test]
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
                EventBus.Default.SetServiceProvider(() =>
                {
                    var serviceProvider = new ServiceProvider();
                    serviceProvider.AddInstance<IHandlerActivator>(new DefaultHandlerActivator());
                    return serviceProvider;
                });
            }
        }

        [Test]
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
                EventBus.Default.SetServiceProvider(() =>
                {
                    var serviceProvider = new ServiceProvider();
                    serviceProvider.AddInstance<IHandlerActivator>(new DefaultHandlerActivator());
                    return serviceProvider;
                });
            }
        }
    }
}
