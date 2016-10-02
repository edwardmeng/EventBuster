using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EventBuster
{
    internal class HandlerActionPool
    {
        private readonly object _lockObject = new object();
        private readonly List<HandlerActionDescriptor> _actionPool = new List<HandlerActionDescriptor>();
        private readonly ConcurrentDictionary<Type, HandlerActionPipeline> _descriptors = new ConcurrentDictionary<Type, HandlerActionPipeline>();

        public void Add(HandlerActionDescriptor descriptor)
        {
            lock (_lockObject)
            {
                if (!_actionPool.Contains(descriptor))
                {
                    _actionPool.Add(descriptor);
                    _descriptors.GetOrAdd(descriptor.EventType, key => new HandlerActionPipeline()).Add(descriptor);
                }
            }
        }

        public bool Remove(HandlerActionDescriptor descriptor)
        {
            lock (_lockObject)
            {
                if (_actionPool.Remove(descriptor))
                {
                    HandlerActionPipeline pipeline;
                    if (_descriptors.TryGetValue(descriptor.EventType, out pipeline))
                    {
                        pipeline.Remove(descriptor);
                        if (pipeline.Count == 0)
                        {
                            _descriptors.TryRemove(descriptor.EventType, out pipeline);
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public HandlerActionPipeline GetPipeline(Type eventType)
        {
            lock (_lockObject)
            {
                HandlerActionPipeline pipeline;
                return _descriptors.TryGetValue(eventType, out pipeline) ? pipeline : new HandlerActionPipeline();
            }
        }
    }
}
