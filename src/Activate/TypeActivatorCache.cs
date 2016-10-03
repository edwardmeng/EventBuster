using System;

namespace EventBuster
{
    internal class TypeActivatorCache
    {
        private readonly Func<Type, ObjectFactory> _createFactory = type => ActivatorUtilities.CreateFactory(type, new Type[0]);
#if Net35
        private readonly System.Collections.Generic.Dictionary<Type, ObjectFactory> _typeActivatorCache = new System.Collections.Generic.Dictionary<Type, ObjectFactory>();
#else
        private readonly System.Collections.Concurrent.ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, ObjectFactory>();
#endif

        public object CreateInstance(IServiceProvider serviceProvider, Type handlerType)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }
            ObjectFactory createFactory;
#if Net35
            lock (_typeActivatorCache)
            {
                if (!_typeActivatorCache.TryGetValue(handlerType,out createFactory))
                {
                    createFactory = _createFactory(handlerType);
                    _typeActivatorCache.Add(handlerType, createFactory);
                }
            }
#else
            createFactory = _typeActivatorCache.GetOrAdd(handlerType, _createFactory);
#endif
            return createFactory(serviceProvider, null);
        }
    }
}
