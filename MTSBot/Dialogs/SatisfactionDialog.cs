using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MTSBot.Dialogs
{
    [Serializable]
    public class SatisfactionDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("You are trying to give a feedback, this feature will colme soon");
            //context.Wait(MessageReceivedAsync);
            context.Done(true);
            //return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;            

            context.Wait(MessageReceivedAsync);
        }
    }
}