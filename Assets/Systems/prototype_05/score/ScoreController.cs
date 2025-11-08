using UnityEngine;

namespace Systems.Prototype_05.Score
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private int totalScore;
        [SerializeField] private int progress;
        [SerializeField] private int packUnlockThreshold;

        private ScoreDatasource datasource = ScoreDatasource.Instance;

        void Update()
        {
            datasource.CurrentScore = progress;
            datasource.OverallScore = totalScore;
            datasource.ScoreToNextDeck = packUnlockThreshold;
        }

        void OnEnable()
        {
            EventBus<ScoreChanged>.Event += HandleScoreEvent;
        }

        private void HandleScoreEvent(ScoreChanged data)
        {
            totalScore += data.Delta;
            if (progress + data.Delta > packUnlockThreshold)
            {
                EventBus<PackUnlockThresholdReached>.Raise();
            }
            progress = totalScore % packUnlockThreshold;
        }
    }
    public struct ScoreChanged : IEvent
    {
        public int Delta;
    }
    public struct PackUnlockThresholdReached : IEvent { }

}