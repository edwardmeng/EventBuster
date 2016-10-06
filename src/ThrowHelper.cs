using System;
using System.Globalization;
using System.Reflection;

namespace EventBuster
{
    internal static class ThrowHelper
    {
        public static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Arg_WrongType, value, targetType), nameof(value));
        }

        public static void ThrowGenericHandlerActionMethodException(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_CannotBeGeneric, method.Name, method.DeclaringType));
        }

        public static void ThrowHandlerActionMethodCannotReturnException(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_CannotReturnValue, method.Name, method.DeclaringType));
        }

        public static void ThrowHandlerActionMethodCannotStaticException(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,Strings.ActionMethod_CannotBeStatic, method.Name, method.DeclaringType));
        }

        public static void ThrowHandlerActionMethodMustBeInputParameterException(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_MustBeInputParameter, method.Name, method.DeclaringType));
        }

        public static void ThrowHandlerActionMethodMustBeSingleParameterException(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_MustBeSingleParameter, method.Name, method.DeclaringType));
        }

        public static void ThrowInvalidAsyncHandlerActionMethodException(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_InvalidAsyncMethod, method.Name, method.DeclaringType));
        }

        public static void ThrowAsyncHandlerActionMethodInvalidFirstParameter(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_AsyncFirstParameter_MustBeEventArgs, method.Name, method.DeclaringType));
        }

        public static void ThrowAsyncHandlerActionMethodInvalidSecondParameter(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ActionMethod_SecondParameter_MustBeToken, method.Name, method.DeclaringType));
        }

        public static void ThrowValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated(Type handlerType, Type activatorType)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated, handlerType.FullName, activatorType.FullName));
        }

        public static void ThrowActivatorCannotResolveParameterException(Type parameterType, Type declareType)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.Activator_CannotResolveParameter, parameterType, declareType));
        }

        public static void ThrowActivatorCannotLocateConstructorException(Type instanceType)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.Activator_CannotLocateConstructor, instanceType));
        }

        public static void ThrowActivatorAmbiguousConstructorsException(Type instanceType)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.Activator_AmbiguousConstructors, instanceType));
        }
    }
}
