using HelperScripts;
using UnityEngine;

public class Moral_Optimism : Moral
{
    public Moral_Optimism() {
        positive = "Optimistisch";
        negative = "Pessimistisch";
        neutral = "Realistisch";
        
        setCase();
    }

    public override float auswerten(VolkObject o, SentimentType sentiment)
    {
        float moraleBoost = 0.1f;
        float moraleLoss = -0.05f;
        float moraleChange = 0f;

        switch (moral)
        {
            case 0: // Optimistic
                moraleChange = sentiment == SentimentType.Positive ? moraleBoost : moraleLoss;
                break;
            case 1: // Realistic
                moraleChange = sentiment == SentimentType.Neutral ? moraleBoost : moraleLoss;
                break;
            case 2: // Pessimistic
                moraleChange = sentiment == SentimentType.Negative ? moraleBoost : moraleLoss;
                break;
        }

        Debug.Log($"[Moral_Optimism] Sentiment: {sentiment}, Moral Case: {moral}, Morale Change: {moraleChange}");
        return moraleChange;
    }
}