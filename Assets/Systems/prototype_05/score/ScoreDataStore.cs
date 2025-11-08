namespace Systems.Prototype_05.Score
{
    public class ScoreDatasource
    {
        public static ScoreDatasource Instance => instance ??= new ScoreDatasource();
        private static ScoreDatasource instance;

        public int OverallScore;
        public int CurrentScore;
        public int ScoreToNextDeck;
    }

    public struct ScoreEvent : IEvent { }
}