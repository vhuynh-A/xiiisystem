using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using xiiiCommonModels;

namespace AzureSmsStorage.Test
{
    internal static class AzureTestUtility
    {
        #region Storage Account
        public static CloudStorageAccount CreateStorageAccount(string connectionString)
        {
            return CloudStorageAccount.Parse(connectionString);
        }
        #endregion

        #region Cloud Table
        public static CloudTable CreateCloudTable(CloudStorageAccount storageAccount, string tableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference(tableName);
        }

        public static CloudTable CreateEmptyCloudTable(CloudStorageAccount storageAccount, string tableName)
        {
            var table = CreateCloudTable(storageAccount, tableName);
            return DeleteCloudTable(table);
        }

        public static CloudTable CreateCloudTable(string connectionString, string tableName)
        {
            var storageAccount = CreateStorageAccount(connectionString);
            return CreateCloudTable(storageAccount, tableName);
        }

        public static CloudTable CreateEmptyCloudTable(string connectionString, string tableName)
        {
            var table = CreateCloudTable(connectionString, tableName);
            return DeleteCloudTable(table);
        }
        public static CloudTable DeleteCloudTable(CloudTable table)
        {
            table.DeleteIfExists();
            return table;
        }
        #endregion

        #region Cloud Queue
        public static CloudQueue CreateCloudQueue(CloudStorageAccount storageAccount, string queueName)
        {
            var queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference(queueName);
        }

        public static CloudQueue CreateEmptyCloudQueue(CloudStorageAccount storageAccount, string queueName)
        {
            var queue = CreateCloudQueue(storageAccount, queueName);
            return DeleteCloudQueue(queue);
        }

        public static CloudQueue CreateCloudQueue(string connectionString, string queueName)
        {
            var storageAccount = CreateStorageAccount(connectionString);
            return CreateCloudQueue(storageAccount, queueName);
        }

        public static CloudQueue CreateEmptyCloudQueue(string connectionString, string queueName)
        {
            var queue = CreateCloudQueue(connectionString, queueName);
            return DeleteCloudQueue(queue);
        }

        public static CloudQueue DeleteCloudQueue(CloudQueue queue)
        {
            queue.DeleteIfExists();
            return queue;
        }
        #endregion

        #region Mock Sms Message
        public static SmsMessage GenerateMockSmsMessage()
        {
            return new SmsMessage
            {
                Created = DateTime.UtcNow,
                From = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid(),
                Message = Guid.NewGuid().ToString(),
                To = Guid.NewGuid().ToString()
            };
        }

        public static bool IsIdentical(SmsMessage a, SmsMessage b)
        {
            if (a.Created != b.Created || a.From != b.From || a.Guid != b.Guid || a.Message != b.Message || a.To != b.To)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsSame(SmsMessage a, SmsMessage b)
        {
            if (a.Created != b.Created || a.From != b.From || a.Message != b.Message || a.To != b.To)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
