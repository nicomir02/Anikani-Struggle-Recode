using UnityEngine;
using UnitySentiment;
using System;
using OverallManager;

namespace HelperScripts
{
    public class SimpleSentimentAnalysis : MonoBehaviour
    {
        public SentimentAnalysis sentimentAnalysis;
        public VoiceRecognition voiceRecognition;

        void Start()
        {
            if (sentimentAnalysis != null)
            {
                sentimentAnalysis.Initialize();
                SentimentAnalysis.OnAnlysisFinished += HandleAnalysisResult;
            }
        }

        void OnDestroy()
        {
            if (sentimentAnalysis != null)
            {
                SentimentAnalysis.OnAnlysisFinished -= HandleAnalysisResult;
            }
        }

        public void AnalyzeSentiment(string text, Action<SentimentResult> onAnalysisComplete)
        {
            if (sentimentAnalysis != null)
            {
                sentimentAnalysis.PredictSentimentText(text);
                _onAnalysisComplete = onAnalysisComplete;
            }
            else
            {
                Debug.LogError("SentimentAnalysis component is not initialized.");
            }
        }

        private Action<SentimentResult> _onAnalysisComplete;

        private void HandleAnalysisResult(Vector3 analysisResult)
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                SentimentResult sentimentResult = new SentimentResult(analysisResult.x, analysisResult.y, analysisResult.z);
                voiceRecognition.ProcessSentimentResult(sentimentResult);
            });
        }
    }
}