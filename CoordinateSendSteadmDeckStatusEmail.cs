using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SteamDeckStatus.Models;

namespace SteamDeckStatus.Function
{
    public class CoordinateSendSteadmDeckStatusEmail
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public CoordinateSendSteadmDeckStatusEmail(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        [FunctionName("CoordinateSendSteadmDeckStatusEmail")]
        public async Task Run([TimerTrigger("0 0 8 * * *")] TimerInfo myTimer, ILogger log, ExecutionContext executionContext)
        {
            var config = new ConfigurationBuilder().SetBasePath(executionContext.FunctionAppDirectory)
                                            .AddJsonFile("local.settings.json", true, true)
                                            .AddEnvironmentVariables()
                                            .Build();

            var getReservationStatusUrl = config["GetSteamDeckStatusFunctionUrl"];
            var uploadEmailMessagesHttpTriggerFunctionUrl = config["UploadEmailMessagesHttpTriggerFunctionUrl"];

            var reservationStatus = await GetReservationStatusAsync(getReservationStatusUrl);

            await PostReservationStatusToEmailQueueFunction(reservationStatus, uploadEmailMessagesHttpTriggerFunctionUrl);
        }

        async Task<ReservationStatus> GetReservationStatusAsync(string steamDeckStatusUrl)
        {

            var result = await _httpClient.GetAsync(steamDeckStatusUrl);

            return await result.Content.ReadFromJsonAsync<ReservationStatus>();
        }

        async Task PostReservationStatusToEmailQueueFunction(ReservationStatus reservationStatus, string uploadEmailMessagesHttpTriggerFunctionUrl)
        {

            var json = JsonConvert.SerializeObject(reservationStatus);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(uploadEmailMessagesHttpTriggerFunctionUrl, data);
        }
    }
}
