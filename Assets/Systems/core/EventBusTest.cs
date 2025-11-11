using Sirenix.OdinInspector;
using UnityEngine;

namespace Systems.Core
{
    public class EventBusTest : MonoBehaviour
    {

        void OnEnable()
        {
            EventBusV2.Subscribe<Event>(HandleEvent, this);
            EventBusV2.Subscribe<Event2>(HandleEvent2, this);
        }

        [Button("Emit Event")]
        public void EmitEvent()
        {
            EventBusV2.Raise(new Event());
        }

        [Button("Emit Event2")]
        public void EmitEvent2()
        {
            EventBusV2.Raise(new Event2()
            {
                Message = "Hello"
            });
        }

        [Button("Unsubscribe all")]
        public void Unsub()
        {
            EventBusV2.UnsubscribeAll(this);
        }
        private void HandleEvent(Event data)
        {
            Debug.Log("Collected Event");
        }

        private void HandleEvent2(Event2 data)
        {
            Debug.Log($"Collected Event2: {data.Message}");
        }

        private struct Event : IEvent { }
        private struct Event2 : IEvent
        {
            public string Message;
        }
    }
}