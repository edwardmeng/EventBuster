EventBuster
===========
Introduce
--------
A lightweight event distribution and consumption for .net applications.

Installing via NuGet!
-----
        Install-Package EventBuster
Supported platform:
------------------
Microsoft .NET Framework 3.5<br/>
Microsoft .NET Framework 4.5.1+<br/>
NetStandard 1.1+

Usage
-----
####Step1 Define strong type event argument

        public class MyEvent
        {
            public int Value{ get; set; }
        }

####Step2 Define event handler action method<br/>
There are six kind legal convension methods, which should be annotated with the EventHandlerAttribute:

* Instance or static method satisfy the following condition:
  1. There is only one input parameter, the type of parameter should be the strong type event argument.
  2. Its return value should be void. 
  
    For example

        public class MyHandlers
        {
            [EventHandler]
            public void MyInstanceAction(MyEvent evt)
            {
                ......
            }

            [EventHandler]
            public static void MyStaticAction(MyEvent evt)
            {
                ......
            }
        }
* Instance or static async method satisfy the following condition:
  1. There is only one input parameter, the type of parameter should be the strong type event argument.
  2. Its return value should be Task. 
  
    For example

        public class MyHandlers
        {
            [EventHandler]
            public async Task MyInstanceAction(MyEvent evt)
            {
                ......
            }

            [EventHandler]
            public static async Task MyStaticAction(MyEvent evt)
            {
                ......
            }
        }
* Instance or static async method satisfy the following condition:
  1. There are only two input parameters, the type of the first parameter should be the strong type event argument, 
     and the type of the second parameter should be CancellationToken.
  2. Its return value should be Task. 
  
    For example

        public class MyHandlers
        {
            [EventHandler]
            public async Task MyInstanceAction(MyEvent evt, CancellationToken token)
            {
                ......
            }

            [EventHandler]
            public static async Task MyStaticAction(MyEvent evt, CancellationToken token)
            {
                ......
            }
        }
####Step3 Register event handlers
* Register handler type via EventBus as the following sample code.

        EventBus.Register<MyHandlers>();

  Then all the event handler action methods annotated with EventHandlerAttribute will be discovered. 
  And the MyHandlers will be instantiated automatically when it is required.
* Register handler instance via EventBus as the following sample code.

        var handler = new MyHandlers();
        EventBus.Register(handler);
  Then all the event handler action methods annotated with EventHandlerAttribute will be discovered. 
  And the registered instance will be used for instance event handler action method invocations directly.
* Register automatically through install nuget package 

        Install-Package EventBuster.Activation
  Then event handlers in all assemblies for the application will be automatically registered while startup.

Advanced
--------
