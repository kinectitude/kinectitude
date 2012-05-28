using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using MessagePassing.Public;

namespace MessagePassing.Core
{
    internal sealed class Entity : Actor, IMessenger, IComponentContainer
    {
        /// <summary>
        /// Used to register a special handler with an event manager which will take ownership of callbacks
        /// and use custom logic to decide when events should be published.
        /// </summary>
        /// <param name="callback">The callback to register for an event</param>
        /// <param name="parameters">Any additional parameters the special event handler may need to perform book-keeping for registered callbacks.</param>
        private delegate void SubscriptionHandler(MessageCallback callback, object[] parameters);

        /// <summary>
        /// Implementation of IEventManager passed to components. Acts as a sandbox.
        /// It prefixes every event published from the component, and allows it to subscribe to other events.
        /// </summary>
        private sealed class PrefixedEventManager : IMessenger
        {
            private readonly string prefix;
            private readonly IMessenger parent;
            private readonly Dictionary<string, SubscriptionHandler> events;
            private readonly Dictionary<string, MessageCallback> actions;
            private readonly Dictionary<string, MessageCallback> callbacks;

            public PrefixedEventManager(Component component, IMessenger parent)
            {
                events = new Dictionary<string, SubscriptionHandler>();
                actions = new Dictionary<string, MessageCallback>();
                callbacks = new Dictionary<string, MessageCallback>();

                Type type = component.GetType();

                this.prefix = type.Name.Replace("Component", string.Empty);
                this.parent = parent;

                // Find any public methods that have [Event] or [Action] attributes

                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    if (Attribute.IsDefined(methodInfo, typeof(EventAttribute)))
                    {
                        string eventName = methodInfo.Name.Replace("Subscribe", string.Empty);
                        events[eventName] = CreateEventSubscriptionHandlerLambda(component, methodInfo);
                    }
                    else if (Attribute.IsDefined(methodInfo, typeof(ActionAttribute)))
                    {
                        actions[methodInfo.Name] = CreateActionLambda(component, methodInfo);
                    }
                }
            }

            /// <summary>
            /// Creates a SubscriptionHandler lambda that accepts a callback, and an array of generic arguments.
            /// The lambda casts the array elements into strongly typed arguments that get passed to the 
            /// actual method.
            /// </summary>
            /// <param name="component">The component instance to call the method on</param>
            /// <param name="methodInfo">The method to call</param>
            /// <returns></returns>
            private SubscriptionHandler CreateEventSubscriptionHandlerLambda(Component component, MethodInfo methodInfo)
            {
                ParameterExpression callback = Expression.Parameter(typeof(MessageCallback));
                ParameterExpression parameters = Expression.Parameter(typeof(object[]));

                // Create an array of expressions representing the casted arguments

                List<Expression> convertedParameters = new List<Expression>();
                convertedParameters.Add(callback);

                int index = 0;
                foreach (ParameterInfo parameterInfo in methodInfo.GetParameters().Skip(1))
                {
                    convertedParameters.Add(Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(index)), parameterInfo.ParameterType));
                    index++;
                }

                return Expression.Lambda<SubscriptionHandler>(
                    Expression.Call(
                        Expression.Constant(component, component.GetType()),
                        methodInfo,
                        convertedParameters
                    ),
                    callback,
                    parameters
                ).Compile();
            }

            /// <summary>
            /// Creates a MessageCallback lambda that accepts an array of generic data. The lambda casts
            /// the elements of the array into strongly typed arguments that get passed to the given method
            /// </summary>
            /// <param name="component">The component instance to call the method on</param>
            /// <param name="methodInfo">The method to call</param>
            /// <returns></returns>
            private MessageCallback CreateActionLambda(Component component, MethodInfo methodInfo)
            {
                ParameterExpression parameters = Expression.Parameter(typeof(object[]));

                List<Expression> convertedParameters = new List<Expression>();

                int index = 0;
                foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                {
                    convertedParameters.Add(Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(index)), parameterInfo.ParameterType));
                    index++;
                }

                return Expression.Lambda<MessageCallback>(
                    Expression.Call(
                        Expression.Constant(component, component.GetType()),
                        methodInfo,
                        convertedParameters
                    ),
                    parameters
                ).Compile();
            }

            /// <summary>
            /// Publish a message from the component. This can either be called by the component (i.e. to indicate a property change)
            /// or by the entity (i.e. to run an action).
            /// </summary>
            /// <param name="id">Unprefixed message id</param>
            /// <param name="data">The data to supply with the message</param>
            public void Publish(string id, params object[] data)
            {
                // First see if the id corresponds to an action

                MessageCallback callback;

                actions.TryGetValue(id, out callback);
                if (null != callback)
                {
                    // Run the one handler for this action
                    callback(data);
                }
                else
                {
                    // This is a message, so publish it to all interested subscribers

                    callbacks.TryGetValue(id, out callback);
                    if (null != callback)
                    {
                        callback(data);
                    }
                }
            }

            /// <summary>
            /// Subscribe to any event available in the entity's scope.
            /// </summary>
            /// <param name="id">The id of the event to subscribe to. May be prefixed with a component name or unprefixed.</param>
            /// <param name="callback">The callback which will be invoked when the event occurs.</param>
            /// <param name="parameters">Any additional parameters to supply with the subscription.</param>
            public void Subscribe(string id, MessageCallback callback, params object[] parameters)
            {
                parent.Subscribe(id, callback, parameters);     // The entity decides what to do
            }

            /// <summary>
            /// Called by the parent entity to store a callback for a prefixed event.
            /// </summary>
            /// <param name="id">The unprefixed event id.</param>
            /// <param name="callback">The callback to store.</param>
            public void LocalSubscribe(string id, MessageCallback callback, object[] parameters)
            {
                // If we got to this point, we could either be subscribing to a special event (i.e. a method with [Event]),
                // or a message which the component happens to publish. Message callbacks go in here. Subscription requests
                // for a special event are forwarded to the specified SubscriptionHandler

                SubscriptionHandler handler;
                events.TryGetValue(id, out handler);

                if (null != handler)
                {
                    handler(callback, parameters);
                }
                else
                {
                    MessageCallback registeredCallback;
                    callbacks.TryGetValue(id, out registeredCallback);

                    if (null == registeredCallback)
                    {
                        registeredCallback = callback;
                    }
                    else
                    {
                        registeredCallback += callback;
                    }

                    callbacks[id] = registeredCallback;
                }
            }
        }

        private Scene scene;
        private readonly List<Component> components;

        public Scene Scene
        {
            get { return scene; }
            set
            {
                if (null == scene)
                {
                    scene = value;
                }
            }
        }

        public Entity()
        {
            components = new List<Component>();
        }

        /// <summary>
        /// Adds the given component, and gives it an event manager and component container.
        /// </summary>
        /// <param name="component">The component to add</param>
        public void AddComponent(Component component)
        {
            component.ComponentContainer = this;
            component.Messenger = new PrefixedEventManager(component, this);
            components.Add(component);
        }

        public T GetManager<T>() where T : class, IManager, new()
        {
            return scene.GetManager<T>();
        }

        private Component GetComponent(string type)
        {
            // TODO: use a dictionary
            return components.FirstOrDefault(x => x.GetType().Name.Replace("Component", string.Empty) == type);
        }

        public void Initialize()
        {
            foreach (Component component in components)
            {
                component.Initialize();
            }
        }

        /// <summary>
        /// Publish either a user message or a prefixed action request.
        /// </summary>
        /// <param name="id">A message id which can be prefixed or unprefixed</param>
        /// <param name="data">Data to supply with the message</param>
        public override void Publish(string id, object[] data)
        {
            Match match = Regex.Match(id, @"^(\w+)\.(\w+)$");

            // If the message has a prefix
            if (match.Success)
            {
                string prefix = match.Groups[1].Value;
                Component component = GetComponent(prefix);

                // If there is a component with this name
                if (null != component)
                {
                    // Have the component's PrefixedEventManager publish to any subscribers it has for this kind of message
                    PrefixedEventManager eventManager = (PrefixedEventManager)component.Messenger;
                    eventManager.Publish(match.Groups[2].Value, data);
                }
            }
            else
            {
                // If the message is not prefixed, it is a normal user message that goes to the scene.
                scene.Publish(id, data);
            }
        }

        /// <summary>
        /// Used to subscribe to either prefixed events from local components, or unprefixed user messages that
        /// can come from anywhere.
        /// </summary>
        /// <param name="id">The id of the message to listen for</param>
        /// <param name="callback">The callback to run when the message is published</param>
        /// <param name="parameters">Any additional book-keeping information an event may need (i.e. Other)</param>
        public override void Subscribe(string id, MessageCallback callback, params object[] parameters)
        {
            if (parameters.Length == 1)
            {
                object[] array = parameters[0] as object[];
                if (null != array)
                {
                    parameters = array;
                }
            }

            Match match = Regex.Match(id, @"^(\w+)\.(\w+)$");

            if (match.Success)
            {
                string prefix = match.Groups[1].Value;
                Component component = GetComponent(prefix);

                if (null != component)
                {
                    PrefixedEventManager eventManager = (PrefixedEventManager)component.Messenger;
                    eventManager.LocalSubscribe(match.Groups[2].Value, callback, parameters);
                }
            }
            else
            {
                match = Regex.Match(id, @"^(\w+)$");

                if (match.Success)
                {
                    scene.Subscribe(id, callback, parameters);
                }
            }
        }
    }
}
