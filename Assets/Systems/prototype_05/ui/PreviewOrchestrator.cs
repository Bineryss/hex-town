using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using System;
using Sirenix.Serialization;
using Systems.Core;

namespace Systems.Prototype_05.UI
{
    public class PreviewOrchestrator : SerializedMonoBehaviour
    {
        [SerializeField] private HexGridGenerator generator;
        [SerializeField] private Camera mainCamera;

        [OdinSerialize] private Dictionary<Guid, IPreviewController> previewControllers;
        //TODO add priority to modes, so that build mode can overite trade mout but not vice versa!

        public VisualElement Root => root;
        private VisualElement root;

        private readonly List<DataIndicator> indicators = new();
        private readonly List<Action> indicatorUpdatePositions = new();
        [SerializeField] private Guid currentMode = Guid.Empty;

        public void Initialize()
        {
            root = new();
            root.pickingMode = PickingMode.Ignore;
            root.style.position = Position.Absolute;
            root.style.top = 0;
            root.style.right = 0;
            root.style.left = 0;
            root.style.bottom = 0;

            foreach (var kvp in previewControllers)
            {
                kvp.Value.Initialize(kvp.Key);
            }

            EventBus<PreviewActivationRequested>.Event += HandlePreviewActivation;
            EventBus<PreviewDeactivationRequested>.Event += HandlePreviewDeactivation;
            EventBus<ScorePreviewRequested>.Event += HandleScorePreview;
        }

        private void HandleScorePreview(ScorePreviewRequested data)
        {
            root.Clear();
            indicators.Clear();
            indicatorUpdatePositions.Clear();
            if (data.Tooltips is null) return;
            foreach (var indicator in data.Tooltips)
            {
                DataIndicator el = new(10, new DataIndicatorDO()
                {
                    points = indicator.Score,
                    icon = indicator.Icon,
                });
                root.Add(el);
                indicatorUpdatePositions.Add(() =>
                {
                    el.Hide();
                    el.Place(WorldToScreen(generator.layout.AxialToWorld(indicator.Position)));
                    el.Show();
                });
            }
        }
        private void HandlePreviewDeactivation(PreviewDeactivationRequested data)
        {
            if (!currentMode.Equals(data.Mode)) return;

            if (previewControllers.TryGetValue(data.Mode, out IPreviewController currentController))
            {
                currentController.Active = false;
            }
            currentMode = Guid.Empty;
        }

        private void HandlePreviewActivation(PreviewActivationRequested data)
        {
            if (!currentMode.Equals(Guid.Empty) && !data.Force) return;

            foreach (var controller in previewControllers.Values)
            {
                controller.Active = false;
            }

            if (previewControllers.TryGetValue(data.Mode, out IPreviewController currentController))
            {
                currentController.Active = true;
            }
            currentMode = data.Mode;
        }

        void LateUpdate()
        {
            foreach (var update in indicatorUpdatePositions)
            {
                update();
            }
        }

        private Vector2 WorldToScreen(Vector3 vector)
        {
            if (mainCamera == null) return new(0, 0);

            Vector3 screenPos = mainCamera.WorldToScreenPoint(vector);
            return new(screenPos.x, screenPos.y);
        }
    }

    public interface IPreviewController
    {
        void Initialize(Guid mode);
        bool Active { get; set; }
        Guid Mode { get; }
    }
    public struct PreviewActivationRequested : IEvent
    {
        public Guid Mode;
        public bool Force;
    }
    public struct PreviewDeactivationRequested : IEvent
    {
        public Guid Mode;
    }
    public struct ScorePreviewRequested : IEvent
    {
        public List<ScorePreview> Tooltips;
    }
    public struct ScorePreview
    {
        public AxialCoordinate Position;
        public string Icon;
        public int Score;
    }
}