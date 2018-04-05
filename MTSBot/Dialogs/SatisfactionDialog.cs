using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MTSBot.Utilits;

namespace MTSBot.Dialogs
{
    [Serializable]
    public class SatisfactionDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // await context.PostAsync("You are trying to give a feedback, this feature will colme soon");
            context.Wait(MessageReceivedAsync);
            // context.Done(true);
            // return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            // double analisysScore = TextAnalyzer.MakeAnalysisRequest(activity.Text);
            String clientResponse = String.Empty;
            if (TextAnalyzer.MakeAnalysisRequest(activity.Text) >= 0.5)
            {
                clientResponse = "Спасибо за отзыв, мы будем работать для Вас еще лучше !";
            }
            else
            {
                clientResponse = "Извините, мы постараемся работать лучше для Вас !";
            }
            await context.PostAsync(clientResponse);
            context.Done(true);
            // context.Wait(MessageReceivedAsync);
        }
    }
}