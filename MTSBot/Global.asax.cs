using Autofac;
using IntermediatorBotSample.Settings;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace MTSBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // To be replaced by table storage

            BotSettings botSettings = new BotSettings();
            string connectionString = botSettings[BotSettings.KeyRoutingDataStorageConnectionString];

            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                    // Bot Storage: register state storage for your bot
                    IBotDataStore<BotData> botDataStore = null;

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        // Default store: volatile in-memory store - Only for prototyping!
                        System.Diagnostics.Debug.WriteLine("WARNING!!! Using InMemoryDataStore, which should be only used for prototyping, for the bot state!");
                        botDataStore = new InMemoryDataStore();
                    }
                    else
                    {
                        // Azure Table Storage
                        System.Diagnostics.Debug.WriteLine("Using Azure Table Storage for the bot state");
                        botDataStore = new TableBotDataStore(connectionString);
                    }

                    builder.Register(c => botDataStore)
                        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                        .AsSelf()
                        .SingleInstance();
                });
        }
    }
}
