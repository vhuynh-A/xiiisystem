using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using System;

namespace AzureSmsStorage.Test
{
    [TestClass]
    public class AzureSmsStorageTest
    {
        [TestMethod]
        public void AzureSmsStorage_WhenStorageDoesNotExist_ShouldCreateStorage()
        {
            // Arrange
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var tableName = string.Format("test{0}", Guid.NewGuid().ToString("N"));
            var queueName = string.Format("test{0}", Guid.NewGuid().ToString("N"));

            var table = AzureTestUtility.CreateEmptyCloudTable(connectionString, tableName);
            var queue = AzureTestUtility.CreateEmptyCloudQueue(connectionString, queueName);

            // Act
            AzureSmsStorage dal = new AzureSmsStorage(table, queue);

            // Assert
            Assert.IsTrue(table.Exists(), "Table was not created.");
            Assert.IsTrue(queue.Exists(), "Queue was not created.");

            // Clean up
            AzureTestUtility.DeleteCloudTable(table);
            AzureTestUtility.DeleteCloudQueue(queue);
        }

        [TestMethod]
        public void AzureSmsStorage_WhenStorageDoesExist_ShouldUseExistingStorage()
        {
            // Arrange
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var tableName = string.Format("test{0}", Guid.NewGuid().ToString("N"));
            var queueName = string.Format("test{0}", Guid.NewGuid().ToString("N"));

            var table = AzureTestUtility.CreateCloudTable(connectionString, tableName);
            var queue = AzureTestUtility.CreateCloudQueue(connectionString, queueName);

            table.CreateIfNotExists();
            queue.CreateIfNotExists();

            // Act
            AzureSmsStorage dal = new AzureSmsStorage(table, queue);

            // Assert
            Assert.AreEqual(tableName, table.Name, "Not using existing table.");
            Assert.AreEqual(queueName, queue.Name, "Not using existing queue.");

            // Clean up
            AzureTestUtility.DeleteCloudTable(table);
            AzureTestUtility.DeleteCloudQueue(queue);
        }

        [TestMethod]
        public void AzureSmsStorage_WithValidRecords_ShouldBeAbleToInsertAndRead()
        {
            // Arrange
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var tableName = string.Format("test{0}", Guid.NewGuid().ToString("N"));
            var queueName = string.Format("test{0}", Guid.NewGuid().ToString("N"));

            var table = AzureTestUtility.CreateCloudTable(connectionString, tableName);
            var queue = AzureTestUtility.CreateCloudQueue(connectionString, queueName);

            var smsMessage = AzureTestUtility.GenerateMockSmsMessage();

            var dal = new AzureSmsStorage(table, queue);

            // Act
            dal.Insert(smsMessage);
            var resultFast = dal.Read(smsMessage.To, smsMessage.Guid);
            var resultSlow = dal.Read(smsMessage.Guid);

            // Assert
            Assert.IsTrue(AzureTestUtility.IsIdentical(smsMessage, resultFast));
            Assert.IsTrue(AzureTestUtility.IsIdentical(smsMessage, resultSlow));

            // Clean up
            AzureTestUtility.DeleteCloudTable(table);
            AzureTestUtility.DeleteCloudQueue(queue);
        }

        [TestMethod]
        public void AzureSmsStorage_WithValidRecords_ShouldBeAbleToInsertAndQueue()
        {
            // Arrange
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var tableName = string.Format("test{0}", Guid.NewGuid().ToString("N"));
            var queueName = string.Format("test{0}", Guid.NewGuid().ToString("N"));

            var table = AzureTestUtility.CreateCloudTable(connectionString, tableName);
            var queue = AzureTestUtility.CreateCloudQueue(connectionString, queueName);

            var smsMessage1 = AzureTestUtility.GenerateMockSmsMessage();
            var smsMessage2 = AzureTestUtility.GenerateMockSmsMessage();

            var dal = new AzureSmsStorage(table, queue);

            // Act
            var insertResult1 = dal.Insert(smsMessage1);
            var queueResult1 = dal.Queue(insertResult1);
            var queueResult2 = dal.InsertThenQueue(smsMessage2);

            queue.FetchAttributes();
            var cachedMessageCount = queue.ApproximateMessageCount;

            // Assert
            Assert.IsTrue(AzureTestUtility.IsIdentical(smsMessage1, queueResult1));
            Assert.IsTrue(AzureTestUtility.IsIdentical(smsMessage2, queueResult2));
            Assert.AreEqual(cachedMessageCount, 2);

            // Clean up
            AzureTestUtility.DeleteCloudTable(table);
            AzureTestUtility.DeleteCloudQueue(queue);
        }

        [TestMethod]
        public void AzureSmsStorage_WithValidRecords_ShouldBeAbleToQueueAndDequeue()
        {
            // Arrange
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var tableName = string.Format("test{0}", Guid.NewGuid().ToString("N"));
            var queueName = string.Format("test{0}", Guid.NewGuid().ToString("N"));

            var table = AzureTestUtility.CreateCloudTable(connectionString, tableName);
            var queue = AzureTestUtility.CreateCloudQueue(connectionString, queueName);

            var smsMessage = AzureTestUtility.GenerateMockSmsMessage();

            var dal = new AzureSmsStorage(table, queue);

            // Act
            var queueResult = dal.InsertThenQueue(smsMessage);
            var dequeueResult = dal.Dequeue();
            dal.Delete(dequeueResult.Guid);

            queue.FetchAttributes();
            var cachedMessageCount = queue.ApproximateMessageCount;

            // Assert
            Assert.IsTrue(AzureTestUtility.IsIdentical(smsMessage, queueResult));
            Assert.IsTrue(AzureTestUtility.IsIdentical(smsMessage, dequeueResult));
            Assert.AreEqual(cachedMessageCount, 0);

            // Clean up
            AzureTestUtility.DeleteCloudTable(table);
            AzureTestUtility.DeleteCloudQueue(queue);
        }
    }
}
