using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using MTSBot.Utilits;

namespace MTSBot.Dialogs
{
    [Serializable]
    public class MTSQADialog : IDialog<object>
    {
        protected int count = 1;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            /*
            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }*/

            try
            {
                string kbId = ConfigurationManager.AppSettings["QnaMaker.KbId"];
                string kbKey = ConfigurationManager.AppSettings["QnaMaker.KbKey"];
                string qnaUrl =
                 $"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{kbId}/generateAnswer";
                HttpClient client = new HttpClient();
                string strtosend = message.Text;
                if (message.ChannelId == "email")
                {
                    var str = strtosend.Split('\n');
                    int maxidx = str.Length;
                    if (maxidx > 3)
                        maxidx = 4;
                    for (int i = 0; i < maxidx; i++)
                        strtosend += str[i] + " ";
                }

                var json = new
                {
                    question = strtosend,
                    top = 3
                };
                var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
                content.Headers.Add("Ocp-Apim-Subscription-Key", kbKey);
                HttpResponseMessage response = await client.PostAsync(qnaUrl, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    QnaResponse qnaResponse =
                        JsonConvert.DeserializeObject<QnaResponse>(await response.Content.ReadAsStringAsync());

                    if (qnaResponse.answers.Count == 0)
                    {
                        await context.PostAsync(
                            "Не могу найти информацию по теме.\n Могу ли еще чем-нибудь помочь из раздела QNA ?");
                        context.Done(true);
                    }
                    else if ((qnaResponse.answers.Count == 1) || (message.ChannelId == "email"))
                    {
                        await context.PostAsync(qnaResponse.answers.First().answer);
                        context.Done(true);
                    }
                    else
                    {
                        qnaResponse.answers.Add(new Answer()
                        {
                            answer = "(exit)",
                            questions = new string[] { "Ничего из предложенного." }
                        });

                        PromptDialog.Choice<Answer>(context, OnSelectedAnswer,
                            qnaResponse.answers,
                            "Это то,что Вы искали ?",
                            descriptions: qnaResponse.answers.Select(x => Tools.UnEscapeXML(x.questions.First())),
                            promptStyle: PromptStyle.Auto
                        );
                    }
                }
                else
                {
                    await context.PostAsync(
                        "Что-то пошло не так. Вам необходимо задать этот вопрос оператору.");
                    context.Done(true);
                }
            }
            catch (Exception e)
            {
                // Write Log
                throw;
            }

        }

        private async Task OnSelectedAnswer(IDialogContext context, IAwaitable<Answer> result)
        {
            Answer answer = await result;
            if (answer.answer != "(exit)")
            {
                await context.PostAsync(answer.answer);
            }
            else
            {
                await context.PostAsync("Извините, не могу найти ответ на вопрос.");
            }

            context.Done(true);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Переустановить счетчик.");
            }
            else
            {
                await context.PostAsync("Не переустанавливать счетчик.");
            }
            context.Wait(MessageReceivedAsync);
        }

        [Serializable]
        class QnaResponse
        {
            public List<Answer> answers { get; set; }
        }

        [Serializable]
        class Answer
        {
            public string answer { get; set; }
            public string[] questions { get; set; }
            public string score { get; set; }

            public override string ToString()
            {
                return questions.First();
            }
        }
    }
}