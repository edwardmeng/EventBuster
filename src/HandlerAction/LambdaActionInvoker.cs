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

        public void Invoke(HandlerActionDescriptor descriptor, object evt)
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
#if !Net35
            else
            {
                System.Threading.Tasks.Task.WaitAll(InvokeAsync(descriptor, evt, System.Threading.CancellationToken.None));
            }
#endif
        }

#if !Net35
        public async System.Threading.Tasks.Task InvokeAsync(HandlerActionDescriptor descriptor, object evt, System.Threading.CancellationToken token)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }
            if (_action != null)
            {
                Invoke(descriptor, evt);
            }
            else
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
                if (_func != null)
                {
                    await _func(eventArgs);
                }
                else if (_func2 != null)
                {
                    await _func2(eventArgs, token);
                }
            }
        }
#endif

        #endregion
    }
}
