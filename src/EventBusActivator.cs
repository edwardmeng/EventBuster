using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MassActivation;

[assembly: AssemblyActivator(typeof(EventBuster.Activation.EventBusActivator))]

namespace EventBuster.Activation
{
    internal class EventBusActivator
    {
        public EventBusActivator(IActivatingEnvironment environment)
        {
            environment.Use(EventBus.Default);
        }

        public void Configuration(IActivatingEnvironment environment, IEventBus eventBus)
        {
            foreach (var assembly in environment.GetAssemblies())
            {
                IEnumerable<Type> types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.TakeWhile(type => type != null);
                }
                foreach (var type in types)
                {
                    eventBus.Register(type);
                }
            }
        }
    }
}
