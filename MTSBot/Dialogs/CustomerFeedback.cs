using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MTSBot.Dialogs
{
    public class CustomerFeedback : TableEntity
    {
        public String Locale { get; set; }
        public String Text { get; set; }
        public double Score { get; set; }

        public CustomerFeedback() { }

        public CustomerFeedback(String pChannelId, String pActivityId)
        {
            this.PartitionKey = pChannelId;
            this.RowKey = pActivityId;            
        }
            
    }
}