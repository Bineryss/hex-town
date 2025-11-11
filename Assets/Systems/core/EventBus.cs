using System;
using System.Collections.Generic;
using System.Linq;

namespace Systems.Core
{
    public interface IEvent { };

    public static class EventBus<T> where T : struct, IEvent
    {
        public static event Action<T> Event;

        public static void Raise(T data = default)
        {
            Event?.Invoke(data);
        }
    }

    public static class EventBusV2
    {
        private static readonly Dictionary<Type, List<SubscriptionInfo>> subscriptions = new();

        private class SubscriptionInfo
        {
            public Delegate Handler { get; }
            public object Owner { get; }

            public SubscriptionInfo(Delegate handler, object owner)
            {
                Handler = handler;
                Owner = owner ?? handler.Target;
            }
        }

        public static void Subscribe<T>(Action<T> handler, object owner = null) where T : struct, IEvent
        {
            var eventType = typeof(T);

            if (!subscriptions.ContainsKey(eventType))
                subscriptions[eventType] = new List<SubscriptionInfo>();

            subscriptions[eventType].Add(new SubscriptionInfo(handler, owner));
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IEvent
        {
            var eventType = typeof(T);
            if (!subscriptions.ContainsKey(eventType)) return;

            subscriptions[eventType].RemoveAll(s =>
                s.Handler.Equals(handler) ||
                (s.Handler.Target == handler.Target && s.Handler.Method == handler.Method));
        }

        public static void UnsubscribeAll(object owner)
        {
            foreach (var subscriptionList in subscriptions.Values)
            {
                subscriptionList.RemoveAll(s => ReferenceEquals(s.Owner, owner));
            }
        }

        public static void Raise<T>(T data = default) where T : struct, IEvent
        {
            var eventType = typeof(T);
            if (!subscriptions.TryGetValue(eventType, out var selectedSubscriptions)) return;

            foreach (var sub in selectedSubscriptions.ToList())
            {
                ((Action<T>)sub.Handler)?.Invoke(data);
            }
        }
    }
}