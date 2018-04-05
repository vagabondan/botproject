using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace MTSBot.Utilits
{
        public static class TextAnalyzer
        {
            public static double MakeAnalysisRequest(string message)
            {
                // Create a client.
                ITextAnalyticsAPI client = new TextAnalyticsAPI
                {
                    AzureRegion = AzureRegions.Westcentralus,
                    SubscriptionKey = Properties.Resources.TextApiSubscriptionKey
                };

                // Extracting sentiment
                SentimentBatchResult result = client.Sentiment(
                        new MultiLanguageBatchInput(
                            new List<MultiLanguageInput>()
                            {
                          new MultiLanguageInput("ru", "0", message),
                            }));
                var score = result.Documents[0].Score;
                return score.HasValue ? score.Value : 0;
                // score.HasValue ? score.ToString() : "Что-то пошло не так";
            }
        }
}