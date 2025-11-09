using UnityEngine;

namespace Systems.Prototype_05.Score
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private int startingUnlockThreshold;
        [SerializeField] private int thresholdIncrease;

        private readonly ScoreDatasource datasource = ScoreDatasource.Instance;

        void OnEnable()
        {
            EventBus<ScoreChanged>.Event += HandleScoreEvent;
            datasource.PackUnlockThreshold = startingUnlockThreshold;
        }

        private void HandleScoreEvent(ScoreChanged data)
        {
            datasource.TotalScore += data.Delta;

            int progress = datasource.Progress + data.Delta;
            while (progress >= datasource.PackUnlockThreshold)
            {
                EventBus<PackUnlockThresholdReached>.Raise();
                progress -= datasource.PackUnlockThreshold;
                datasource.PackUnlockThreshold += thresholdIncrease;
            }
            datasource.Progress = progress;
        }
    }
    public struct ScoreChanged : IEvent
    {
        public int Delta;
    }
    public struct PackUnlockThresholdReached : IEvent { }

}