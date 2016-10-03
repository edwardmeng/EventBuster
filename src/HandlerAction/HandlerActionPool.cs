using System;
using System.Collections.Generic;

namespace EventBuster
{
    internal class HandlerActionPool
    {
        private readonly object _lockObject = new object();
        private readonly List<HandlerActionDescriptor> _actionPool = new List<HandlerActionDescriptor>();
#if Net35
        private readonly Dictionary<Type, HandlerActionPipeline> _descriptors = new Dictionary<Type, HandlerActionPipeline>();
#else
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Type, HandlerActionPipeline> _descriptors = new System.Collections.Concurrent.ConcurrentDictionary<Type, HandlerActionPipeline>();
#endif

        public void Add(HandlerActionDescriptor descriptor)
        {
            lock (_lockObject)
            {
                if (!_actionPool.Contains(descriptor))
                {
                    _actionPool.Add(descriptor);
#if Net35
                    HandlerActionPipeline pipeline;
                    if (!_descriptors.TryGetValue(descriptor.EventType,out pipeline))
                    {
                        pipeline = new HandlerActionPipeline();
                        _descriptors.Add(descriptor.EventType, pipeline);
                    }
                    pipeline.Add(descriptor);
#else
                    _descriptors.GetOrAdd(descriptor.EventType, key => new HandlerActionPipeline()).Add(descriptor);
#endif
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
#if Net35
                            _descriptors.Remove(descriptor.EventType);
#else
                            _descriptors.TryRemove(descriptor.EventType, out pipeline);
#endif
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
