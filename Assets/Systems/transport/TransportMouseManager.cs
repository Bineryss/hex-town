using System;
using RTSCamera;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Transport
{
    public class TransportMouseManager : SerializedMonoBehaviour
    {
        public TransportManager transportController;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private WorldNode a;
        [SerializeField] private WorldNode b;
        [OdinSerialize] private Guid selectedRouteId;



        private RTSCameraInputs inputActions;
        void Awake()
        {
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
            inputActions ??= new RTSCameraInputs();
            inputActions.Enable();

            Keyboard.current.onTextInput += ctx => HandleKeyPressed(ctx);
        }

        private void HandleKeyPressed(char ctx)
        {
            if (ctx == 't')
            {
                TransportRoute route = transportController.CreateRoute(a, b);
                if (route == null)
                {
                    Debug.Log("Route creation failed");
                    return;
                }
                selectedRouteId = route.id;
            }
            if (ctx == 'z')
            {
                if (selectedRouteId == Guid.Empty) return;
                transportController.RemoveRoute(selectedRouteId);
                selectedRouteId = Guid.Empty;
            }
        }
    }
}