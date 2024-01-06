namespace HelperScripts
{
    public class SentimentResult
    {
        public float PositivePercentage { get; private set; }
        public float NegativePercentage { get; private set; }
        public float NeutralPercentage { get; private set; }

        // Constructor
        public SentimentResult(float positive, float negative, float neutral)
        {
            PositivePercentage = positive;
            NegativePercentage = negative;
            NeutralPercentage = neutral;
        }

        public SentimentType GetDominantSentiment()
        {
            if (PositivePercentage > NegativePercentage && PositivePercentage > NeutralPercentage)
                return SentimentType.Positive;
            else if (NegativePercentage > PositivePercentage && NegativePercentage > NeutralPercentage)
                return SentimentType.Negative;
            else
                return SentimentType.Neutral;
        }
    }

    public enum SentimentType
    {
        Positive,
        Negative,
        Neutral
    }
}
