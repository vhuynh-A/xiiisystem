using AzureSmsStorage.Models;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using xiiiCommonDals;
using xiiiCommonModels;

namespace AzureSmsStorage
{
    public class AzureSmsStorage : ISmsRepository
    {
        private CloudTable _table;
        private CloudQueue _queue;

        private Dictionary<Guid, CloudQueueMessage> _dequeuedMessages;

        public AzureSmsStorage(CloudTable table, CloudQueue queue)
        {
            _table = table;
            _queue = queue;

            _table.CreateIfNotExists();
            _queue.CreateIfNotExists();

            _dequeuedMessages = new Dictionary<Guid, CloudQueueMessage>();
        }

        public SmsMessage Insert(SmsMessage message)
        {
            if (message.Guid == Guid.Empty)
            {
                message.Guid = Guid.NewGuid();
            }

            var entity = new SmsMessageEntity(message);
            var insertOperation = TableOperation.Insert(entity);
            _table.Execute(insertOperation);

            return message;
        }

        public SmsMessage Read(Guid guid)
        {
            var rowKey = guid.ToString();
            
            var query = (from entity in _table.CreateQuery<SmsMessageEntity>()
                         where entity.RowKey == rowKey
                         select entity).FirstOrDefault();

            var result = query.Model();
            return result;
        }

        public SmsMessage Read(string to, Guid guid)
        {
            var paritionKey = to;
            var rowKey = guid.ToString();

            var retrieveOperation = TableOperation.Retrieve<SmsMessageEntity>(paritionKey, rowKey);
            var retrievedResult = _table.Execute(retrieveOperation);

            if (retrievedResult == null)
            {
                return null;
            }

            var castedResult = retrievedResult.Result as SmsMessageEntity;
            var result = castedResult.Model();
            return result;
        }

        public SmsMessage Queue(SmsMessage message)
        {
            if (message.Guid == Guid.Empty)
            {
                throw new ArgumentException("guid");
            }

            var retrievedResult = Read(message.To, message.Guid);

            if (retrievedResult == null)
            {
                throw new InvalidOperationException("message not found in table");
            }

            var tableReference = new AzureTableReference { To = message.To, Guid = message.Guid };
            var entity = new CloudQueueMessage(tableReference.Reference);
            _queue.AddMessage(entity);

            return message;
        }

        public SmsMessage InsertThenQueue(SmsMessage message)
        {
            var insertResult = Insert(message);
            var queueResult = Queue(insertResult);
            return queueResult;
        }

        public SmsMessage Dequeue()
        {
            var retrievedMessage = _queue.GetMessage();

            var tableReferenceString = retrievedMessage.AsString;
            var tableReference = new AzureTableReference { Reference = tableReferenceString };

            var message = Read(tableReference.To, tableReference.Guid);

            _dequeuedMessages.Add(message.Guid, retrievedMessage);

            return message;
        }

        public void Delete(Guid smsGuid)
        {
            var message = _dequeuedMessages[smsGuid];
            _queue.DeleteMessage(message);
        }
    }
}
