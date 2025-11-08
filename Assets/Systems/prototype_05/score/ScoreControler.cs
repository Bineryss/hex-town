using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Systems.Prototype_05.Score
{
    public class ScoreControler : MonoBehaviour
    {
        [SerializeField] private int overallScore;
        [SerializeField] private int currentScore;
        [SerializeField] private int scoreRequiredForNextPack;

        private ScoreDatasource datasource = ScoreDatasource.Instance;
        private List<Test> testEventQueue = new();

        void Update()
        {
            datasource.CurrentScore = currentScore;
            datasource.OverallScore = overallScore;
            datasource.ScoreToNextDeck = scoreRequiredForNextPack;
            EventBus<ScoreEvent>.Raise(new ScoreEvent());
        }

        [Button("Trigger test event")]

        public void InvokeTest()
        {
            EventBus<Test>.Raise(new Test() { message = "Hello" });
        }

        void OnEnable()
        {
            EventBus<ScoreEvent>.Event += handleScoreEvent;
            EventBus<Test>.Event += handleTestEvent;

        }

        private void handleScoreEvent(ScoreEvent data)
        {
            Debug.Log("Score event called");
        }

        private void handleTestEvent(Test data)
        {
            testEventQueue.Add(data);
            Debug.Log($"Score event called {data.message}, event called {testEventQueue.Count} times");
        }

        private struct Test : IEvent
        {
            public string message;
        }

    }
}