using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class ScoreIndicator : VisualElement
    {
        private Label overallScoreLabel = new();
        private Label currentScoreLabel = new();
        private Label pointsUntilNextLabel = new();

        public ScoreIndicator(ScoreIndicatorDO data)
        {
            style.backgroundColor = Colors.PRIMARY;
            style.color = Colors.TEXT;
            style.fontSize = 24;
            style.color = Colors.TEXT;
            style.alignItems = Align.Center;
            style.alignSelf = Align.Center;
            style.borderTopLeftRadius = 100;
            style.borderTopRightRadius = 100;
            style.borderBottomLeftRadius = 100;
            style.borderBottomRightRadius = 100;
            style.height = 150;
            style.width = 150;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.Center;

            Add(overallScoreLabel);
            overallScoreLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            overallScoreLabel.style.fontSize = 24;
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = 0;
            Add(container);
            container.Add(currentScoreLabel);
            currentScoreLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            currentScoreLabel.style.fontSize = 24;
            currentScoreLabel.style.paddingRight = 0;
            currentScoreLabel.style.marginRight = 0;
            container.Add(pointsUntilNextLabel);
            pointsUntilNextLabel.style.fontSize = 24;
            currentScoreLabel.style.paddingLeft = 0;
            currentScoreLabel.style.marginLeft = 0;

            Update(data);
        }

        public void Update(ScoreIndicatorDO data)
        {
            overallScoreLabel.text = data.overallScore.ToString();
            currentScoreLabel.text = $"{data.currentScore}/";
            pointsUntilNextLabel.text = data.pointsUntilNext.ToString();
        }
    }

    public struct ScoreIndicatorDO
    {
        public int overallScore;
        public int currentScore;
        public int pointsUntilNext;
    }
}
