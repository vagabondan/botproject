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
            WhatToDo.Add("help", "Задать вопрос");
            WhatToDo.Add("feedback", "Оценить работу");
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            // return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Ask User about what he wants
            PromptDialog.Choice<string>(context, OnSelectedAnswer,
                WhatToDo.Values,
                "Что Вы хотите сделать ?",
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
                    await context.PostAsync("Задайте пожалуйста вопрос, чтобы мы смогли помочь Вам");
                    context.Call(new MTSQADialog(), Resume);
                }
                else if (WhatToDo["feedback"] == message)
                {
                    await context.PostAsync("Оцените пожалуйста, смогли ли мы помочь Вам");
                    context.Call(new SatisfactionDialog(), Resume);
                }

            }
            else
            {
                await context.PostAsync("Извините, не понимаю, что Вы хотите сделать");
                context.Wait(MessageReceivedAsync);
            }

        }

        private async Task Resume(IDialogContext context, IAwaitable<object> result)
        {

            await context.PostAsync("Спасибо, если понадобится помощь, зовите.");
            PromptDialog.Choice<string>(context, OnSelectedAnswer,
                WhatToDo.Values,
                "Что Вы хотите сделать ?",
                promptStyle: PromptStyle.Auto
            );

        }
    }
}