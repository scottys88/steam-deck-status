using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SendGrid.Helpers.Mail;

namespace SteamDeckStatus.Function
{
    public class SendGridEmailQueueTriggerFunction
    {
        [FunctionName("SendGridEmailQueueTriggerFunction")]
        public void Run([QueueTrigger("email-queue", Connection = "CloudStorageAccount")] string myQueueItem, [SendGrid(ApiKey = "SendgridAPIKey")] out SendGridMessage sendGridMessage, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            try
            {
                var queueItem = myQueueItem.ToString();

                dynamic jsonData = JObject.Parse(queueItem);
                string emailBody = jsonData.Content;

                sendGridMessage = new SendGridMessage
                {
                    From = new EmailAddress("scott_schubert@hotmail.com.au", "Steam Deck Status"),
                };
                sendGridMessage.AddTo("scott_schubert@hotmail.com.au");
                sendGridMessage.SetSubject("Awesome Azure Function app");
                sendGridMessage.AddContent("text/html", emailBody);
            }
            catch (Exception ex)
            {
                sendGridMessage = new SendGridMessage();
                log.LogError($"Error occured while processing QueueItem {myQueueItem} , Exception - {ex.InnerException}");

            }

        }
    }
}
