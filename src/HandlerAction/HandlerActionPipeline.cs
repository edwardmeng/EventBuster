using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuster
{
    internal class HandlerActionPipeline : IEnumerable<HandlerActionDescriptor>
    {
        private readonly object _lockObject = new object();
        private readonly List<HandlerActionDescriptor>[] _stages;

        public HandlerActionPipeline()
        {
            _stages = new List<HandlerActionDescriptor>[typeof(HandlerPriority).GetFields(BindingFlags.Public | BindingFlags.Static).Length];
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

        public void Invoke(object evt)
        {
            foreach (var actionDescriptor in this)
            {
                actionDescriptor.Invoker.Invoke(actionDescriptor, evt);
            }
        }

#if !Net35
        public async Task InvokeAsync(object evt, CancellationToken token)
        {
            foreach (var actionDescriptor in this)
            {
                await actionDescriptor.Invoker.InvokeAsync(actionDescriptor, evt, token);
            }
        }
#endif
    }
}
