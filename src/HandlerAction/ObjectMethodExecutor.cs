using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventBuster
{
    internal class ObjectMethodExecutor
    {
        private ActionExecutor _executor;

#if !Net35
        
        private ActionExecutorAsync _executorAsync;
        private static readonly MethodInfo _convertOfTMethod =
            typeof(ObjectMethodExecutor).GetRuntimeMethods().Single(methodInfo => methodInfo.Name == nameof(Convert));

        private static readonly Expression<Func<object, System.Threading.Tasks.Task<object>>> _createTaskFromResultExpression = result => System.Threading.Tasks.Task.FromResult(result);

        private static readonly MethodInfo _createTaskFromResultMethod = ((MethodCallExpression)_createTaskFromResultExpression.Body).Method;

        private static readonly Expression<Func<object, string, Type, System.Threading.Tasks.Task<object>>> _coerceTaskExpression =
            (result, methodName, declaringType) => CoerceTaskType(result, methodName, declaringType);

        private static readonly MethodInfo _coerceMethod = ((MethodCallExpression)_coerceTaskExpression.Body).Method;

#endif

        private ObjectMethodExecutor(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            MethodInfo = methodInfo;
        }

        private delegate object ActionExecutor(object target, object[] parameters);

        private delegate void VoidActionExecutor(object target, object[] parameters);

        public MethodInfo MethodInfo { get; }

        public static ObjectMethodExecutor Create(MethodInfo methodInfo, Type targetType)
        {
            return new ObjectMethodExecutor(methodInfo)
            {
                _executor = GetExecutor(methodInfo, targetType)
#if !Net35
                ,_executorAsync = GetExecutorAsync(methodInfo, targetType)
#endif
            };
        }

        public object Execute(object target, object[] parameters)
        {
            return _executor(target, parameters);
        }

        private static ActionExecutor GetExecutor(MethodInfo methodInfo, Type targetType)
        {
            // Parameters to executor
            var targetParameter = Expression.Parameter(typeof(object), "target");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            var parameters = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                var paramInfo = paramInfos[i];
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            // Call method
            MethodCallExpression methodCall;
            if (methodInfo.IsStatic)
            {
                methodCall = Expression.Call(methodInfo, parameters.ToArray());
            }
            else
            {
                var instanceCast = Expression.Convert(targetParameter, targetType);
                methodCall = Expression.Call(instanceCast, methodInfo, parameters);
            }

            // methodCall is "((Ttarget) target) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<VoidActionExecutor>(methodCall, targetParameter, parametersParameter);
                var voidExecutor = lambda.Compile();
                return WrapVoidAction(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutor signature
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<ActionExecutor>(castMethodCall, targetParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        private static ActionExecutor WrapVoidAction(VoidActionExecutor executor)
        {
            return delegate (object target, object[] parameters)
            {
                executor(target, parameters);
                return null;
            };
        }

#if !Net35

        private delegate System.Threading.Tasks.Task<object> ActionExecutorAsync(object target, object[] parameters);
        
        public System.Threading.Tasks.Task<object> ExecuteAsync(object target, object[] parameters)
        {
            return _executorAsync(target, parameters);
        }

        private static ActionExecutorAsync GetExecutorAsync(MethodInfo methodInfo, Type targetType)
        {
            // Parameters to executor
            var targetParameter = Expression.Parameter(typeof(object), "target");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            var parameters = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                var paramInfo = paramInfos[i];
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            // Call method
            MethodCallExpression methodCall;
            if (methodInfo.IsStatic)
            {
                methodCall = Expression.Call(methodInfo, parameters);
            }
            else
            {
                var instanceCast = Expression.Convert(targetParameter, targetType);
                methodCall = Expression.Call(instanceCast, methodInfo, parameters);
            }

            // methodCall is "((Ttarget) target) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<VoidActionExecutor>(methodCall, targetParameter, parametersParameter);
                var voidExecutor = lambda.Compile();
                return WrapVoidActionAsync(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutorAsync signature
                var coerceMethodCall = GetCoerceMethodCallExpression(methodCall, methodInfo);
                var lambda = Expression.Lambda<ActionExecutorAsync>(coerceMethodCall, targetParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        // We need to CoerceResult as the object value returned from methodInfo.Invoke has to be cast to a Task<T>.
        // This is necessary to enable calling await on the returned task.
        // i.e we need to write the following var result = await (Task<ActualType>)mInfo.Invoke.
        // Returning Task<object> enables us to await on the result.
        private static Expression GetCoerceMethodCallExpression(MethodCallExpression methodCall, MethodInfo methodInfo)
        {
            var castMethodCall = Expression.Convert(methodCall, typeof(object));
            var returnType = methodCall.Type;
#if NetCore
            var isAsync = typeof(System.Threading.Tasks.Task).GetTypeInfo().IsAssignableFrom(returnType.GetTypeInfo());
#else
            var isAsync = typeof(System.Threading.Tasks.Task).IsAssignableFrom(returnType);
#endif
            if (isAsync)
            {
                if (returnType == typeof(System.Threading.Tasks.Task))
                {
                    var stringExpression = Expression.Constant(methodInfo.Name);
                    var typeExpression = Expression.Constant(methodInfo.DeclaringType);
                    return Expression.Call(null, _coerceMethod, castMethodCall, stringExpression, typeExpression);
                }

                var taskValueType = GetTaskInnerTypeOrNull(returnType);
                if (taskValueType != null)
                {
                    // for: public Task<T> Action()
                    // constructs: return (Task<object>)Convert<T>((Task<T>)result)
                    var genericMethodInfo = _convertOfTMethod.MakeGenericMethod(taskValueType);
                    var genericMethodCall = Expression.Call(null, genericMethodInfo, castMethodCall);
                    var convertedResult = Expression.Convert(genericMethodCall, typeof(System.Threading.Tasks.Task<object>));
                    return convertedResult;
                }

                // This will be the case for types which have derived from Task and Task<T>
                throw new InvalidOperationException(string.Format(Strings.ActionExecutor_UnexpectedTaskInstance, methodInfo.Name, methodInfo.DeclaringType));
            }

            return Expression.Call(null, _createTaskFromResultMethod, castMethodCall);
        }

        private static ActionExecutorAsync WrapVoidActionAsync(VoidActionExecutor executor)
        {
            return delegate (object target, object[] parameters)
            {
                executor(target, parameters);
                return System.Threading.Tasks.Task.FromResult<object>(null);
            };
        }

        private static System.Threading.Tasks.Task<object> CoerceTaskType(object result, string methodName, Type declaringType)
        {
            var resultAsTask = (System.Threading.Tasks.Task)result;
            return CastToObject(resultAsTask);
        }

        /// <summary>
        /// Cast Task to Task of object
        /// </summary>
        private static async System.Threading.Tasks.Task<object> CastToObject(System.Threading.Tasks.Task task)
        {
            await task;
            return null;
        }

        /// <summary>
        /// Cast Task of T to Task of object
        /// </summary>
        private static async System.Threading.Tasks.Task<object> CastToObject<T>(System.Threading.Tasks.Task<T> task)
        {
            return await task as object;
        }

        private static Type GetTaskInnerTypeOrNull(Type type)
        {
            var genericType = ClosedGenericMatcher.ExtractGenericInterface(type, typeof(System.Threading.Tasks.Task<>));

            return genericType?.GenericTypeArguments[0];
        }

        private static System.Threading.Tasks.Task<object> Convert<T>(object taskAsObject)
        {
            var task = (System.Threading.Tasks.Task<T>)taskAsObject;
            return CastToObject(task);
        }
#endif
    }
}
