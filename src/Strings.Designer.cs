﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventBuster {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EventBuster.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 The method &apos;{0}&apos; on type &apos;{1}&apos; returned a Task instance even though it is not an asynchronous method. 的本地化字符串。
        /// </summary>
        internal static string ActionExecutor_UnexpectedTaskInstance {
            get {
                return ResourceManager.GetString("ActionExecutor_UnexpectedTaskInstance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The first parameter of async event handler method &apos;{0}&apos; on type &apos;{1}&apos; must be the event argument. 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_AsyncFirstParameter_MustBeEventArgs {
            get {
                return ResourceManager.GetString("ActionMethod_AsyncFirstParameter_MustBeEventArgs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The event handler method &apos;{0}&apos; on type &apos;{1}&apos; cannot be contains generic parameters. 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_CannotBeGeneric {
            get {
                return ResourceManager.GetString("ActionMethod_CannotBeGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The return value of event handler method &apos;{0}&apos; on type &apos;{1}&apos; should be void or Task. 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_CannotReturnValue {
            get {
                return ResourceManager.GetString("ActionMethod_CannotReturnValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The async event handler method &apos;{0}&apos; on type &apos;{1}&apos; should be declared as following:
        ///[EventHandler]
        ///public Task MyHandler(SomeEvent evt)
        ///{
        ///    ......
        ///}
        ///- or -
        ///[EventHandler]
        ///public Task MyHandler(SomeEvent evt, CancellationToken token)
        ///{
        ///    ......
        ///} 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_InvalidAsyncMethod {
            get {
                return ResourceManager.GetString("ActionMethod_InvalidAsyncMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The event handler method &apos;{0}&apos; on type &apos;{1}&apos; cannot have out or ref parameters. 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_MustBeInputParameter {
            get {
                return ResourceManager.GetString("ActionMethod_MustBeInputParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The event handler method &apos;{0}&apos; on type &apos;{1}&apos; must have single input parameter. 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_MustBeSingleParameter {
            get {
                return ResourceManager.GetString("ActionMethod_MustBeSingleParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The second parameter of async event handler method &apos;{0}&apos; on type &apos;{1}&apos; must be the CancellationToken. 的本地化字符串。
        /// </summary>
        internal static string ActionMethod_SecondParameter_MustBeToken {
            get {
                return ResourceManager.GetString("ActionMethod_SecondParameter_MustBeToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value &quot;{0}&quot; is not of type &quot;{1}&quot; and cannot be used in this event handler. 的本地化字符串。
        /// </summary>
        internal static string Arg_WrongType {
            get {
                return ResourceManager.GetString("Arg_WrongType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The service provider for the event bus must be set. 的本地化字符串。
        /// </summary>
        internal static string ProviderNotSetMessage {
            get {
                return ResourceManager.GetString("ProviderNotSetMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The type &apos;{0}&apos; cannot be activated by &apos;{1}&apos; because it is either a value type, an interface, an abstract class or an open generic type. 的本地化字符串。
        /// </summary>
        internal static string ValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated {
            get {
                return ResourceManager.GetString("ValueInterfaceAbstractOrOpenGenericTypesCannotBeActivated", resourceCulture);
            }
        }
    }
}
