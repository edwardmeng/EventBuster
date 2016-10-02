using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

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
            _targetType = targetType ?? methodInfo.ReflectedType;
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
            _targetType = targetInstance?.GetType() ?? _methodInfo.ReflectedType;
            _executor = ObjectMethodExecutor.Create(methodInfo, _targetType);
            EventType = methodInfo.GetParameters()[0].ParameterType;
        }

        #endregion

        #region Implementation

        public Type EventType { get; }

        public void Invoke(HandlerActionDescriptor descriptor, object evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
#if !Net35
            if (typeof(Task).IsAssignableFrom(_methodInfo.ReturnType))
            {
                Task.WaitAll(InvokeAsync(descriptor, evt, CancellationToken.None));
            }
            else
#endif
            {
                if (!EventType.IsInstanceOfType(evt))
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(evt, EventType);
                }
                using (var transaction = descriptor.CreateTransactionScope(TransactionScopeAsyncFlowOption.Suppress))
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
                    transaction?.Complete();
                }
            }
        }

#if !Net35
        public async Task InvokeAsync(HandlerActionDescriptor descriptor, object evt, CancellationToken token)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            token.ThrowIfCancellationRequested();
            if (!typeof(Task).IsAssignableFrom(_methodInfo.ReturnType))
            {
                Invoke(descriptor, evt);
            }
            else
            {
                if (!EventType.IsInstanceOfType(evt))
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(evt, EventType);
                }
                using (var transaction = descriptor.CreateTransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var parameters = _methodInfo.GetParameters();
                    var arguments = new List<object> { evt };
                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(CancellationToken))
                    {
                        arguments.Add(token);
                    }
                    if (!_methodInfo.IsStatic)
                    {
                        if (_targetInstance != null)
                        {
                            await (Task)_executor.Execute(_targetInstance, arguments.ToArray());
                        }
                        else
                        {
                            var activator = (IHandlerActivator)descriptor.Services.GetService(typeof(IHandlerActivator));
                            var instance = activator.Create(descriptor.Services, _targetType);
                            try
                            {
                                await (Task)_executor.Execute(instance, arguments.ToArray());
                            }
                            finally
                            {
                                activator.Release(instance);
                            }
                        }
                    }
                    else
                    {
                        await (Task)_executor.Execute(null, arguments.ToArray());
                    }
                    transaction?.Complete();
                }
            }
        }
#endif
        #endregion
    }
}
