using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuster
{
    internal class LambdaActionInvoker<TEvent> : IHandlerActionInvoker
    {
        #region Fields

        private readonly Action<TEvent> _action;
        private readonly Func<TEvent, Task> _func;
        private readonly Func<TEvent, CancellationToken, Task> _func2;

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

        public LambdaActionInvoker(Func<TEvent, Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            _func = func;
        }

        public LambdaActionInvoker(Func<TEvent, CancellationToken, Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            _func2 = func;
        }

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
            else
            {
                Task.WaitAll(InvokeAsync(descriptor, evt, CancellationToken.None));
            }
        }

        public async Task InvokeAsync(HandlerActionDescriptor descriptor, object evt, CancellationToken token)
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
 
        #endregion
    }
}
