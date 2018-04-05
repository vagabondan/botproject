using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MTSBot.Dialogs
{
    [Serializable]
    public class QADialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("You are trying to ask for help, this feature will colme soon");

            //context.Wait(MessageReceivedAsync);
            context.Done(true);
            //return Task.CompletedTask;
        }

        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    var activity = await result as IMessageActivity;

        //    // TODO: Put logic for handling user message here
            

        //    context.Wait(MessageReceivedAsync);
        //}
    }
}