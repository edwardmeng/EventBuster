using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    internal class HandlerActionPipeline : IEnumerable<HandlerActionDescriptor>
    {
        private readonly object _lockObject = new object();
        private readonly List<HandlerActionDescriptor>[] _stages;

        public HandlerActionPipeline()
        {
#if NetCore
            _stages = new List<HandlerActionDescriptor>[typeof(HandlerPriority).GetTypeInfo().DeclaredFields.Count(field => field.IsStatic && field.IsPublic)];
#else
            _stages = new List<HandlerActionDescriptor>[typeof(HandlerPriority).GetFields(BindingFlags.Public | BindingFlags.Static).Length];
#endif
            for (var i = 0; i < _stages.Length; ++i)
            {
                _stages[i] = new List<HandlerActionDescriptor>();
            }
        }

        public HandlerActionPipeline(IEnumerable<HandlerActionDescriptor> descriptors) : this()
        {
            if (descriptors != null)
            {
                lock (_lockObject)
                {
                    foreach (var descriptor in descriptors)
                    {
                        AddInternal(descriptor);
                    }
                }
            }
        }

        public IEnumerator<HandlerActionDescriptor> GetEnumerator()
        {
            var stages = new List<HandlerActionDescriptor>();
            lock (_lockObject)
            {
                stages.AddRange(_stages.SelectMany(stage => stage));
            }
            return stages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get; private set; }

        public void Remove(HandlerActionDescriptor descriptor)
        {
            lock (_lockObject)
            {
                if (_stages[Convert.ToInt32(descriptor.Priority)].Remove(descriptor))
                {
                    Count--;
                }
            }
        }

        public void Add(HandlerActionDescriptor descriptor)
        {
            lock (_lockObject)
            {
                AddInternal(descriptor);
            }
        }

        private void AddInternal(HandlerActionDescriptor descriptor)
        {
            _stages[Convert.ToInt32(descriptor.Priority)].Add(descriptor);
            Count++;
        }

        public void Invoke(IServiceProvider serviceProvider, IDictionary<Type, object> instancePool, object evt)
        {
#if !Net35
            lock (_lockObject)
            {
                foreach (var stage in _stages)
                {
                    var tasks = new List<System.Threading.Tasks.Task>();
                    foreach (var actionDescriptor in stage)
                    {
                        var context = new HandlerActionContext(instancePool, actionDescriptor, serviceProvider);
                        if (actionDescriptor.Invoker.IsAsync)
                        {
                            tasks.Add(actionDescriptor.Invoker.InvokeAsync(context, evt, System.Threading.CancellationToken.None));
                        }
                        else
                        {
                            actionDescriptor.Invoker.Invoke(context, evt);
                        }
                    }
                    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                }
            }
#else
            foreach (var actionDescriptor in this)
            {
                actionDescriptor.Invoker.Invoke(new HandlerActionContext(instancePool, actionDescriptor, serviceProvider), evt);
            }
#endif
        }

#if !Net35
        public async System.Threading.Tasks.Task InvokeAsync(IServiceProvider serviceProvider, IDictionary<Type, object> instancePool, object evt, System.Threading.CancellationToken token)
        {
            foreach (var actionDescriptor in this)
            {
                var context = new HandlerActionContext(instancePool, actionDescriptor, serviceProvider);
                if (actionDescriptor.Invoker.IsAsync)
                {
                    await actionDescriptor.Invoker.InvokeAsync(context, evt, token);
                }
                else
                {
                    actionDescriptor.Invoker.Invoke(context,evt);
                }
            }
        }
#endif
    }
}
