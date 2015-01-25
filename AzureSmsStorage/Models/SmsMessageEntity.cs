using Microsoft.WindowsAzure.Storage.Table;
using System;
using xiiiCommonModels;

namespace AzureSmsStorage.Models
{
    internal class SmsMessageEntity : TableEntity
    {
        public SmsMessageEntity()
        {
        }

        public SmsMessageEntity(string to, string guid)
        {
            this.PartitionKey = to;
            this.RowKey = guid;
        }

        public SmsMessageEntity(SmsMessage model)
        {
            this.PartitionKey = model.To;
            this.RowKey = model.Guid.ToString();
            Created = model.Created;
            From = model.From;
            Message = model.Message;
        }

        public string From { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }

        public SmsMessage Model()
        {
            var model = new SmsMessage
            {
                Created = this.Created,
                From = this.From,
                Guid = Guid.Parse(this.RowKey),
                Message = this.Message,
                To = this.PartitionKey
            };

            return model;
        }
    }
}
