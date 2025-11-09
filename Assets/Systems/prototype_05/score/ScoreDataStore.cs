namespace Systems.Prototype_05.Score
{
    public class ScoreDatasource
    {
        public static ScoreDatasource Instance => instance ??= new ScoreDatasource();
        private static ScoreDatasource instance;

        public int TotalScore;
        public int Progress;
        public int PackUnlockThreshold;
    }
}