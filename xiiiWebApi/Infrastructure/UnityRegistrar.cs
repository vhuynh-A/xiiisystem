using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using xiiiCommonDals;

namespace xiiiWebApi.Infrastructure
{
    internal class UnityRegistrar
    {
        public UnityContainer Register(UnityContainer container)
        {
            if (container == null)
            {
                container = new UnityContainer();
            }

            // Register all types here.
            var accountStorage = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudTableClient smsTableClient = accountStorage.CreateCloudTableClient();
            CloudTable smsTable = smsTableClient.GetTableReference(CloudConfigurationManager.GetSetting("SmsStorageName"));
            CloudQueueClient smsQueueClient = accountStorage.CreateCloudQueueClient();
            CloudQueue smsQueue = smsQueueClient.GetQueueReference(CloudConfigurationManager.GetSetting("SmsStorageName"));

            container.RegisterType<ISmsRepository, AzureSmsStorage.AzureSmsStorage>(new InjectionConstructor(smsTable, smsQueue));

            return container;
        }
    }
}