using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using SteamDeckStatus.Models;

namespace SteamDeckStatus.Function
{
    public class GetSteamDeckStatus
    {
        private readonly IConfiguration _configuration;

        public GetSteamDeckStatus(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("GetSteamDeckStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string SteamDeckStatusUrl = _configuration["SteamDeckStatusUrl"];
            string cannotReserveSearchString = _configuration["CannotReserveString"];

            try
            {
                var httpClient = new HttpClient();

                var result = await httpClient.GetAsync(SteamDeckStatusUrl);

                if (result.IsSuccessStatusCode)
                {
                    var reservationResponseRoot = await result.Content.ReadFromJsonAsync<ReservationResponseRoot>();
                    var reservationStatus = new ReservationStatus()
                    {
                        CanBeReserved = false
                    };

                    if (reservationResponseRoot?.strReservationMessage != null)
                    {
                        if (reservationResponseRoot.strReservationMessage.Contains(cannotReserveSearchString))
                        {
                            return new OkObjectResult(reservationStatus);
                        }

                        reservationStatus.CanBeReserved = true;
                        return new OkObjectResult(reservationStatus);
                    }
                }

                return new BadRequestResult();

            }
            catch
            {
                return new BadRequestResult();

            }


        }
    }
}
