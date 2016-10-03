using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventBuster
{
    /// <summary>
    /// The default implementation of <see cref="IServiceProvider"/>.
    /// </summary>
    internal class ServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, List<Func<object>>> _priorServices;
        private readonly IDictionary<Type, Func<object>> _services;

        public ServiceProvider()
        {
            _services = new Dictionary<Type, Func<object>>();
            _priorServices = new Dictionary<Type, List<Func<object>>>();
            _services[typeof(IServiceProvider)] = () => this;
        }

        public virtual ServiceProvider Add(Type serviceType, Func<object> serviceFactory)
        {
            Func<object> func;
            if (_services.TryGetValue(serviceType, out func))
            {
                List<Func<object>> list;
                if (_priorServices.TryGetValue(serviceType, out list))
                {
                    list.Add(func);
                }
                else
                {
                    list = new List<Func<object>> { func };
                    _priorServices.Add(serviceType, list);
                }
            }
            _services[serviceType] = serviceFactory;
            return this;
        }

        public virtual ServiceProvider Add(Type serviceType, Type implementationType)
        {
            var factory = ActivatorUtilities.CreateFactory(implementationType, new Type[0]);
            return Add(serviceType, () => factory(this,new object[0]));
        }

        public virtual ServiceProvider AddInstance<TService>(object instance)
        {
            return AddInstance(typeof(TService), instance);
        }

        public virtual ServiceProvider AddInstance(Type service, object instance)
        {
            return Add(service, () => instance);
        }

        private object GetMultiService(Type collectionType)
        {
            Func<object> func;
#if NetCore
            var reflectingCollectionType = collectionType.GetTypeInfo();
#else
            var reflectingCollectionType = collectionType;
#endif
            if (!reflectingCollectionType.IsGenericType || reflectingCollectionType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                return null;
            }
#if NetCore
            var key = reflectingCollectionType.GenericTypeArguments.Single();
#else
            var key = collectionType.GetGenericArguments().Single();
#endif
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(key));
            if (_services.TryGetValue(key, out func))
            {
                List<Func<object>> list2;
                list.Add(func());
                if (!_priorServices.TryGetValue(key, out list2))
                {
                    return list;
                }
                foreach (var func2 in list2)
                {
                    list.Add(func2());
                }
            }
            return list;
        }

        private object GetSingleService(Type serviceType)
        {
            Func<object> func;
            return !_services.TryGetValue(serviceType, out func) ? null : func();
        }

        public virtual object GetService(Type serviceType)
        {
            return GetSingleService(serviceType) ?? GetMultiService(serviceType);
        }

        public virtual ServiceProvider RemoveAll<T>()
        {
            return RemoveAll(typeof(T));
        }

        public virtual ServiceProvider RemoveAll(Type type)
        {
            _services.Remove(type);
            _priorServices.Remove(type);
            return this;
        }

    }
}
