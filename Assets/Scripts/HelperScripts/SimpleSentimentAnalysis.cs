using UnityEngine;
using SentimentAnalysis = UnitySentiment.SentimentAnalysis; // Use the correct namespace if different

namespace HelperScripts
{
    public class SimpleSentimentAnalysis : MonoBehaviour
    {
        public SentimentAnalysis sentimentAnalysis;
        public VoiceRecognition voiceRecognition;
    
        void Start()
        {
            voiceRecognition = FindObjectOfType<VoiceRecognition>();
            if(sentimentAnalysis != null)
            {
                sentimentAnalysis.Initialize();
                SentimentAnalysis.OnAnlysisFinished += HandleAnalysisFinished;
            }
        }
    
        void OnDestroy()
        {
            if(sentimentAnalysis != null)
            {
                SentimentAnalysis.OnAnlysisFinished -= HandleAnalysisFinished;
            }
        }
    
        public void AnalyzeSentiment(string text)
        {
            sentimentAnalysis.PredictSentimentText(text);
        }
    
        private void HandleAnalysisFinished(Vector3 analysisResult)
        {
            // Convert the result to SentimentResult and process it
            SentimentResult sentimentResult = new SentimentResult(analysisResult.x, analysisResult.y, analysisResult.z);
            // Here, you might want to pass sentimentResult to some method that handles the next steps.
            // For example:
            ProcessSentimentResult(sentimentResult);
        }
    
        private void ProcessSentimentResult(SentimentResult sentimentResult)
        {
            // This is an example. Adapt this logic to fit your game's needs.
            switch (sentimentResult.GetDominantSentiment())
            {
                case SentimentType.Positive:
                    // Handle positive sentiment
                    voiceRecognition.HandlePositiveSentiment();
                    break;
                case SentimentType.Negative:
                    // Handle negative sentiment
                    voiceRecognition.HandleNegativeSentiment();
                    break;
                case SentimentType.Neutral:
                    // Handle neutral sentiment
                    voiceRecognition.HandleNeutralSentiment();
                    break;
            }
        }
    }
}


