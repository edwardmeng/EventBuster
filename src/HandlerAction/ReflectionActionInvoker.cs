using System;
using System.Reflection;

namespace EventBuster
{
    internal class ReflectionActionInvoker : IHandlerActionInvoker
    {
        #region Fields

        private readonly MethodInfo _methodInfo;
        private readonly Type _targetType;
        private readonly object _targetInstance;
        private readonly ObjectMethodExecutor _executor;

        #endregion

        #region Constructors

        public ReflectionActionInvoker(MethodInfo methodInfo, Type targetType = null)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            _methodInfo = methodInfo;
            if (targetType != null)
            {
                _targetType = targetType;
            }
            else
            {
#if NetCore
                _targetType = methodInfo.DeclaringType;
#else
                _targetType = methodInfo.ReflectedType;
#endif
            }
            _executor = ObjectMethodExecutor.Create(methodInfo, targetType);
            EventType = methodInfo.GetParameters()[0].ParameterType;
        }

        public ReflectionActionInvoker(MethodInfo methodInfo, object targetInstance = null)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            _methodInfo = methodInfo;
            _targetInstance = targetInstance;
            if (targetInstance != null)
            {
                _targetType = targetInstance.GetType();
            }
            else
            {
#if NetCore
                _targetType = methodInfo.DeclaringType;
#else
                _targetType = methodInfo.ReflectedType;
#endif
            }
            _executor = ObjectMethodExecutor.Create(methodInfo, _targetType);
            EventType = methodInfo.GetParameters()[0].ParameterType;
        }

        #endregion

        #region Implementation

        public Type EventType { get; }

#if !Net35
        private bool IsAsyncMethod(MethodInfo method)
        {
#if NetCore
            return typeof(System.Threading.Tasks.Task).GetTypeInfo().IsAssignableFrom(method.ReturnType.GetTypeInfo());
#else
            return typeof(System.Threading.Tasks.Task).IsAssignableFrom(method.ReturnType);
#endif
        }

#endif

        private void ValidateEvent(object evt)
        {
#if NetCore
            if (!EventType.GetTypeInfo().IsAssignableFrom(evt.GetType().GetTypeInfo()))
#else
            if (!EventType.IsInstanceOfType(evt))
#endif
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(evt, EventType);
            }
        }

        public void Invoke(HandlerActionDescriptor descriptor, object evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
#if !Net35
            if (IsAsyncMethod(_methodInfo))
            {
                System.Threading.Tasks.Task.WaitAll(InvokeAsync(descriptor, evt, System.Threading.CancellationToken.None));
            }
            else
#endif
            {
                ValidateEvent(evt);
                using (var transaction =
#if Net35
                    descriptor.CreateTransactionScope()
#elif Net451
                    descriptor.CreateTransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Suppress)
#else
                    (IDisposable)null
#endif
                    )
                {
                    if (!_methodInfo.IsStatic)
                    {
                        if (_targetInstance != null)
                        {
                            _executor.Execute(_targetInstance, new[] { evt });
                        }
                        else
                        {
                            var activator = (IHandlerActivator)descriptor.Services.GetService(typeof(IHandlerActivator));
                            var instance = activator.Create(descriptor.Services, _targetType);
                            try
                            {
                                _executor.Execute(instance, new[] { evt });
                            }
                            finally
                            {
                                activator.Release(instance);
                            }
                        }
                    }
                    else
                    {
                        _executor.Execute(null, new[] { evt });
                    }
#if !NetCore
                    transaction?.Complete();
#endif
                }
            }
        }

#if !Net35
        public async System.Threading.Tasks.Task InvokeAsync(HandlerActionDescriptor descriptor, object evt, System.Threading.CancellationToken token)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            token.ThrowIfCancellationRequested();
            if (!IsAsyncMethod(_methodInfo))
            {
                Invoke(descriptor, evt);
            }
            else
            {
                ValidateEvent(evt);
                using (var transaction =
#if NetCore
                    (IDisposable)null
#else
                    descriptor.CreateTransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled)
#endif
                    )
                {
                    var parameters = _methodInfo.GetParameters();
                    var arguments = new System.Collections.Generic.List<object> { evt };
                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(System.Threading.CancellationToken))
                    {
                        arguments.Add(token);
                    }
                    if (!_methodInfo.IsStatic)
                    {
                        if (_targetInstance != null)
                        {
                            await (System.Threading.Tasks.Task)_executor.Execute(_targetInstance, arguments.ToArray());
                        }
                        else
                        {
                            var activator = (IHandlerActivator)descriptor.Services.GetService(typeof(IHandlerActivator));
                            var instance = activator.Create(descriptor.Services, _targetType);
                            try
                            {
                                await (System.Threading.Tasks.Task)_executor.Execute(instance, arguments.ToArray());
                            }
                            finally
                            {
                                activator.Release(instance);
                            }
                        }
                    }
                    else
                    {
                        await (System.Threading.Tasks.Task)_executor.Execute(null, arguments.ToArray());
                    }
#if !NetCore
                    transaction?.Complete();
#endif
                }
            }
        }
#endif
        #endregion
    }
}
