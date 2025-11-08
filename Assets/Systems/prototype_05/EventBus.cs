using System;

namespace Systems.Prototype_05
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
}