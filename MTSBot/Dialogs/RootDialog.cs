using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MTSBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private Dictionary<string, string> WhatToDo = new Dictionary<string, string>();

        public RootDialog()
        {
            WhatToDo.Add("help", "Get help");
            WhatToDo.Add("feedback", "Give feedback");
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            //return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Ask User about what he wants
            PromptDialog.Choice<string>(context, OnSelectedAnswer,
                WhatToDo.Values,
                "What do you want to do?",
                promptStyle: PromptStyle.Auto
            );
        }

        private async Task OnSelectedAnswer(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;

            if (WhatToDo.ContainsValue(message))
            {

                if (WhatToDo["help"] == message)
                {
                    context.Call(new QADialog(), Resume);
                }
                else if (WhatToDo["feedback"] == message)
                {
                    context.Call(new SatisfactionDialog(), Resume);
                }

            }
            else
            {
                await context.PostAsync("sorry, I don't understand what you want to go");
                context.Wait(MessageReceivedAsync);
            }

        }

        private async Task Resume(IDialogContext context, IAwaitable<object> result)
        {

            await context.PostAsync("Thanks you, let me know what else I can do for you.");
            PromptDialog.Choice<string>(context, OnSelectedAnswer,
                WhatToDo.Values,
                "What do you want to do?",
                promptStyle: PromptStyle.Auto
            );

        }
    }
}