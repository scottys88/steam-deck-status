using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Extensions.Configuration;

namespace SteamDeckStatus.Function
{
    public static class UploadEmailMessagesHttpTriggerFunction
    {
        [FunctionName("UploadEmailMessagesHttpTriggerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
      log.LogInformation($"C# Http trigger function executed at: {DateTime.Now}");  

            CreateQueueIfNotExists(log, context);  

            for (int i = 1; i <= 5; i++)  
            {  
                string randomStr = Guid.NewGuid().ToString();  
                var serializeJsonObject = JsonConvert.SerializeObject(
                                             new { 
                                                   ID = randomStr, 
                                                   Content = $"<html><body><h2> This is a Sample HTML content {i}! </h2></body></html>"
                                                 });  
  
                CloudStorageAccount storageAccount = GetCloudStorageAccount(context);
                CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();  
                CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("email-queue");  
                var cloudQueueMessage = new CloudQueueMessage(serializeJsonObject); 
 
                await cloudQueue.AddMessageAsync(cloudQueueMessage);  
            }  
  
            return new OkObjectResult("UploadEmailMessagesHttpTriggerFunction executed successfully!!");  
        }  
 
        private static void CreateQueueIfNotExists(ILogger logger, ExecutionContext executionContext)  
        {  
            CloudStorageAccount storageAccount = GetCloudStorageAccount(executionContext);  
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();  
            string[] queues = new string[] { "email-queue" };  
            foreach (var item in queues)  
            {  
                CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(item);  
                cloudQueue.CreateIfNotExistsAsync(); 
            }  
        }  
        private static CloudStorageAccount GetCloudStorageAccount(ExecutionContext executionContext)  
        {  
            var config = new ConfigurationBuilder().SetBasePath(executionContext.FunctionAppDirectory)  
                                                   .AddJsonFile("local.settings.json", true, true)
                                                   .AddEnvironmentVariables()
                                                   .Build();  

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["CloudStorageAccount"]);  
            return storageAccount;  
        } 
    }
}