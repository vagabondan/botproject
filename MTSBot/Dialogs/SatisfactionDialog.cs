using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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
            double feedbackScore = TextAnalyzer.MakeAnalysisRequest(activity.Text);
            String clientResponse = String.Format("Ваш отзыв был оценен как положительный на {0}%.\n{1}",
                Math.Round(feedbackScore * 100),
                (feedbackScore >= 0.5) ?
                "Спасибо, мы будем работать для Вас еще лучше !" :
                "Извините, мы постараемся работать для Вас лучше !");
                // (feedbackScore >= 0.5) ?
                // "Спасибо за отзыв, мы будем работать для Вас еще лучше !" :
                // "Извините, мы постараемся работать лучше для Вас !";
            
            // ---------------------------------------------------------------
            // Save customer's reply to Azure Storage Tables
            // ---------------------------------------------------------------
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // ("DefaultEndpointsProtocol=https;AccountName=tevappstorage;AccountKey=NMr/WodMMGKGijaOBFV4xqVMKL8UlZbUDWB608W5XazIuYX4UMf4FHf3r2rZtNL7PdBEIWmoRsOLt8JiRW9YHw==;EndpointSuffix=core.windows.net");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("CustomerFeedback");
            table.CreateIfNotExists();

            CustomerFeedback custFeedback = new CustomerFeedback(activity.ChannelId, activity.Id);
            custFeedback.Locale = activity.Locale;
            custFeedback.Text = activity.Text;
            custFeedback.Score = feedbackScore;

            TableOperation insertOperation = TableOperation.Insert(custFeedback);
            await table.ExecuteAsync(insertOperation);

            await context.PostAsync(clientResponse);
            context.Done(true);
            // context.Wait(MessageReceivedAsync);
        }
    }
}