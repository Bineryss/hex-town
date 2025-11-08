using System.Collections.Generic;
using Systems.Prototype_05.Score;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    [ExecuteInEditMode]
    public class UIController : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform target;

        private VisualElement container;
        private DataIndicator dataIndicator;
        private ScoreIndicator scoreIndicator;
        private ScoreDatasource scoreDatasource = ScoreDatasource.Instance; //TODO add service locator

        private void BuildUI(VisualElement root)
        {
            VisualElement footer = new();
            footer.style.alignSelf = Align.FlexEnd;
            footer.style.paddingBottom = 32;
            footer.style.paddingRight = 32;
            footer.style.paddingLeft = 32;
            footer.style.flexDirection = FlexDirection.Row;
            footer.style.justifyContent = Justify.SpaceBetween;
            footer.style.flexGrow = 1;
            container = new();
            container.Add(footer);
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.FlexEnd;
            container.style.flexGrow = 1;
            container.style.alignContent = Align.Stretch;
            dataIndicator = new DataIndicator(new DataIndicatorDO()
            {
                points = 10,
                icon = "W"
            });
            root.Add(dataIndicator);
            root.Add(container);
            scoreIndicator = new ScoreIndicator(new ScoreIndicatorDO()
            {
                overallScore = 564,
                currentScore = 4,
                pointsUntilNext = 90
            });
            footer.Add(scoreIndicator);
            footer.Add(new InventoryContainer(new List<InventoryElementDO>()
            {
                new()
                {
                    icon = "WHEAT FIELD",
                    quantity = 3
                },
                new()
                {
                    icon = "FARM HOUSE",
                    quantity = 2
                }
            }));
            footer.Add(new VisualElement());
        }

        void Update()
        {
            scoreIndicator.Update(new ScoreIndicatorDO()
            {
                currentScore = scoreDatasource.CurrentScore,
                overallScore = scoreDatasource.OverallScore,
                pointsUntilNext = scoreDatasource.ScoreToNextDeck
            });
            dataIndicator.Place(WorldToScreen(target.position));
        }

        private Vector2 WorldToScreen(Vector3 vector)
        {
            if (mainCamera == null) return new(0, 0);

            Vector3 screenPos = mainCamera.WorldToScreenPoint(vector);
            return new(screenPos.x, screenPos.y);
        }

        private void Setup()
        {
            if (document == null) return;
            VisualElement root = document.rootVisualElement;
            if (root == null)
            {
                root = new();
            }
            root.Clear();
            BuildUI(root);
        }

        void OnEnable()
        {
            Setup();
        }
        void OnValidate()
        {
            Setup();
        }
    }
}