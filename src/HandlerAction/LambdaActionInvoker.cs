using System;

namespace EventBuster
{
    internal class LambdaActionInvoker<TEvent> : IHandlerActionInvoker
    {

        #region Fields

        private readonly Action<TEvent> _action;
#if !Net35
        private readonly Func<TEvent, System.Threading.Tasks.Task> _func;
        private readonly Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> _func2;
#endif

        #endregion

        #region Constructors

        public LambdaActionInvoker(Action<TEvent> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            _action = action;
        }

#if !Net35
        public LambdaActionInvoker(Func<TEvent, System.Threading.Tasks.Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            _func = func;
        }

        public LambdaActionInvoker(Func<TEvent, System.Threading.CancellationToken, System.Threading.Tasks.Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            _func2 = func;
        }
#endif

        #endregion

        #region Implementation

        public Type EventType => typeof(TEvent);

        public void Invoke(HandlerActionContext context, object evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            if (_action != null)
            {
                TEvent eventArgs;
                try
                {
                    eventArgs = (TEvent)evt;
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(evt, typeof(TEvent));
                    return;
                }
                _action.Invoke(eventArgs);
            }
        }

#if !Net35

        public bool IsAsync => _func != null || _func2 != null;

        public async System.Threading.Tasks.Task InvokeAsync(HandlerActionContext context, object evt, System.Threading.CancellationToken token)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            TEvent eventArgs;
            try
            {
                eventArgs = (TEvent)evt;
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(evt, typeof(TEvent));
                return;
            }
            if (_func != null)
            {
                await _func(eventArgs);
            }
            else if (_func2 != null)
            {
                await _func2(eventArgs, token);
            }
        }
#endif

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as LambdaActionInvoker<TEvent>;
            if (other == null) return false;
            if (!Equals(_action, other._action)) return false;
#if !Net35
            return Equals(_func, other._func) && Equals(_func2, other._func2);
#else
            return true;
#endif
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _action?.GetHashCode() ?? 0;
#if !Net35
                hashCode = (hashCode * 397) ^ (_func?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (_func2?.GetHashCode() ?? 0);
#endif
                return hashCode;
            }
        }

        #endregion
    }
}
