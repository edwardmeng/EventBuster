using System;
using System.Collections.Concurrent;

namespace EventBuster
{
    internal class TypeActivatorCache
    {
        private readonly Func<Type, ObjectFactory> _createFactory =
            type => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
        private readonly ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache =
               new ConcurrentDictionary<Type, ObjectFactory>();

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

            var createFactory = _typeActivatorCache.GetOrAdd(handlerType, _createFactory);
            return createFactory(serviceProvider, null);
        }
    }
}
