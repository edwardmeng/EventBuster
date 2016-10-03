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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as ReflectionActionInvoker;
            if (other == null) return false;
            if (!Equals(_methodInfo, other._methodInfo)) return false;
            return _targetInstance != null ? Equals(_targetInstance, other._targetInstance) : _targetType == other._targetType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _methodInfo.GetHashCode();
                if (_targetInstance != null)
                {
                    hashCode = (hashCode * 397) ^ _targetInstance.GetHashCode();
                }
                else
                {
                    hashCode = (hashCode * 397) ^ _targetType.GetHashCode();
                }
                return hashCode;
            }
        }

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

        public void Invoke(HandlerActionContext context, object evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            ValidateEvent(evt);
            using (var transaction =
#if Net35
                    context.CreateTransactionScope()
#elif Net451
                    context.CreateTransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Suppress)
#else
                    (IDisposable)null
#endif
                    )
            {
                _executor.Execute(_methodInfo.IsStatic ? null : (_targetInstance ?? context.GetInstance(_targetType)), new[] { evt });
#if !NetCore
                transaction?.Complete();
#endif
            }
        }

#if !Net35

        public bool IsAsync
        {
            get
            {
#if NetCore
            return typeof(System.Threading.Tasks.Task).GetTypeInfo().IsAssignableFrom(_methodInfo.ReturnType.GetTypeInfo());
#else
                return typeof(System.Threading.Tasks.Task).IsAssignableFrom(_methodInfo.ReturnType);
#endif
            }
        }

        public async System.Threading.Tasks.Task InvokeAsync(HandlerActionContext context, object evt, System.Threading.CancellationToken token)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            token.ThrowIfCancellationRequested();
            ValidateEvent(evt);
            using (var transaction =
#if NetCore
                    (IDisposable)null
#else
                    context.CreateTransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled)
#endif
                    )
            {
                var parameters = _methodInfo.GetParameters();
                var arguments = new System.Collections.Generic.List<object> { evt };
                if (parameters.Length == 2 && parameters[1].ParameterType == typeof(System.Threading.CancellationToken))
                {
                    arguments.Add(token);
                }
                await (System.Threading.Tasks.Task)_executor.Execute(_methodInfo.IsStatic ? null : (_targetInstance ?? context.GetInstance(_targetType)), arguments.ToArray());
#if !NetCore
                transaction?.Complete();
#endif
            }
        }
#endif
        #endregion
    }
}
